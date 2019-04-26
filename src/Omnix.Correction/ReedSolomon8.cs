using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using Omnix.Base;
using Omnix.Base.Extensions;
using Omnix.Correction.Internal;

namespace Omnix.Correction
{
    public class ReedSolomon8
    {
        private readonly int _k;
        private readonly int _n;
        private readonly BufferPool _bufferPool;
        private readonly byte[] _encMatrix;

        public ReedSolomon8(int k, int n, BufferPool bufferPool)
        {
            _k = k;
            _n = n;
            _bufferPool = bufferPool;
            _encMatrix = ReadSolomonMath.CreateEncodeMatrix(k, n);
        }

        internal static void LoadNativeMethods()
        {
            ReadSolomonMath.LoadNativeMethods();
        }

        internal static void LoadPureUnsafeMethods()
        {
            ReadSolomonMath.LoadPureUnsafeMethods();
        }

        public async Task Encode(ReadOnlyMemory<byte>[] sources, int[] index, Memory<byte>[] repairs, int packetLength, int concurrency = 1, CancellationToken token = default)
        {
            if (sources == null) throw new ArgumentNullException(nameof(sources));
            if (repairs == null) throw new ArgumentNullException(nameof(repairs));
            if (index == null) throw new ArgumentNullException(nameof(index));

            await Enumerable.Range(0, repairs.Length).ForEachAsync(row =>
            {
                return Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();

                    // *remember* indices start at 0, k starts at 1.
                    if (index[row] < _k)
                    {
                        // < k, systematic so direct copy.
                        BytesOperations.Copy(sources[index[row]].Span, repairs[row].Span, packetLength);
                    }
                    else
                    {
                        // index[row] >= k && index[row] < n
                        int pos = index[row] * _k;
                        BytesOperations.Zero(repairs[row].Span.Slice(packetLength));

                        for (int col = 0; col < _k; col++)
                        {
                            token.ThrowIfCancellationRequested();

                            ReadSolomonMath.AddMul(sources[col].Span, repairs[row].Span, _encMatrix[pos + col], packetLength);
                        }
                    }
                });
            }, concurrency, token, false);
        }

        public async Task Decode(Memory<byte>[] packets, int[] index, int packetLength, int concurrency = 1, CancellationToken token = default)
        {
            if (packets == null) throw new ArgumentNullException(nameof(packets));
            if (index == null) throw new ArgumentNullException(nameof(index));

            Shuffle(packets, index, _k);

            var decMatrix = ReadSolomonMath.CreateDecodeMatrix(_encMatrix, index, _k, _n);

            // do the actual decoding..
            var tempPackets = new byte[_k][];

            await Enumerable.Range(0, _k).ForEachAsync(row =>
            {
                return Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();

                    if (index[row] >= _k)
                    {
                        tempPackets[row] = _bufferPool.GetArrayPool().Rent(packetLength);
                        BytesOperations.Zero(tempPackets[row].AsSpan(0, packetLength));

                        for (int col = 0; col < _k; col++)
                        {
                            token.ThrowIfCancellationRequested();

                            ReadSolomonMath.AddMul(packets[col].Span, tempPackets[row], decMatrix[row * _k + col], packetLength);
                        }
                    }
                });
            }, concurrency, token, false);

            token.ThrowIfCancellationRequested();

            // move pkts to their final destination
            for (int row = 0; row < _k; row++)
            {
                if (index[row] >= _k)
                {
                    // only copy those actually decoded.
                    BytesOperations.Copy(tempPackets[row], packets[row].Span, packetLength);
                    index[row] = row;
                    _bufferPool.GetArrayPool().Return(tempPackets[row]);
                }
            }
        }

        private static void Shuffle(Memory<byte>[] packets, int[] index, int k)
        {
            for (int i = 0; i < k;)
            {
                if (index[i] >= k || index[i] == i)
                {
                    i++;
                }
                else
                {
                    // put pkts in the right position (first check for conflicts).
                    int c = index[i];

                    if (index[c] == c)
                    {
                        throw new ArgumentException("Shuffle error at " + i);
                    }

                    // swap(index[c],index[i])
                    int tmp = index[i];
                    index[i] = index[c];
                    index[c] = tmp;

                    // swap(pkts[c],pkts[i])
                    var tmp2 = packets[i];
                    packets[i] = packets[c];
                    packets[c] = tmp2;
                }
            }
        }

        private static unsafe class ReadSolomonMath
        {
            private static NativeLibraryManager? _nativeLibraryManager;

            private delegate void MulDelegate(byte* src, byte* dst, byte* table, int len);

            private static MulDelegate _mul;

            /**
             * Primitive polynomials - see Lin & Costello, Appendix A,
             * and Lee & Messerschmitt, p. 453.
             */
            private static string[] _prim_polys = {
                                          // gfBits         polynomial
                "",                       // 0              no code
                "",                       // 1              no code
                "111",                    // 2              1+x+x^2
                "1101",                   // 3              1+x+x^3
                "11001",                  // 4              1+x+x^4
                "101001",                 // 5              1+x^2+x^5
                "1100001",                // 6              1+x+x^6
                "10010001",               // 7              1+x^3+x^7
                "101110001",              // 8              1+x^2+x^3+x^4+x^8
                "1000100001",             // 9              1+x^4+x^9
                "10010000001",            // 10             1+x^3+x^10
                "101000000001",           // 11             1+x^2+x^11
                "1100101000001",          // 12             1+x+x^4+x^6+x^12
                "11011000000001",         // 13             1+x+x^3+x^4+x^13
                "110000100010001",        // 14             1+x+x^6+x^10+x^14
                "1100000000000001",       // 15             1+x+x^15
                "11010000000010001"       // 16             1+x+x^3+x^12+x^16
            };

            private static readonly int _gfBits = 8;
            private static readonly int _gfSize;

            /**
             * To speed up computations, we have tables for logarithm, exponent
             * and inverse of a number. If gfBits &lt;= 8, we use a table for
             * multiplication as well (it takes 64K, no big deal even on a PDA,
             * especially because it can be pre-initialized an put into a ROM!),
             * otherwhise we use a table of logarithms.
             */

            // index->poly form conversion table
            private static byte[] _gf_exp;
            // Poly->index form conversion table
            private static int[] _gf_log;
            // inverse of field elem.
            private static byte[] _inverse;

            /**
             * gf_mul(x,y) multiplies two numbers. If gfBits &lt;= 8, it is much
             * faster to use a multiplication table.
             *
             * USE_GF_MULC, GF_MULC0(c) and GF_ADDMULC(x) can be used when multiplying
             * many numbers by the same constant. In this case the first
             * call sets the constant, and others perform the multiplications.
             * A value related to the multiplication is held in a local variable
             * declared with USE_GF_MULC . See usage in addMul1().
             */
            private static byte[][] _gf_mul_table;

            static ReadSolomonMath()
            {
                try
                {
                    LoadNativeMethods();
                }
                catch (Exception)
                {
                    LoadPureUnsafeMethods();
                }

                _gfSize = ((1 << _gfBits) - 1);

                _gf_exp = new byte[2 * _gfSize];
                _gf_log = new int[_gfSize + 1];
                _inverse = new byte[_gfSize + 1];

                GenerateGF();
                InitMulTable();
            }

            internal static void LoadNativeMethods()
            {
                _nativeLibraryManager?.Dispose();

                try
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        if (Environment.Is64BitProcess)
                        {
                            _nativeLibraryManager = new NativeLibraryManager("Assemblies/Omnix.Correction.win-x64.dll");
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                        {
                            _nativeLibraryManager = new NativeLibraryManager("Assemblies/Omnix.Correction.linux-x64.so");
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                    _mul = _nativeLibraryManager.GetMethod<MulDelegate>("mul");
                }
                catch (Exception)
                {
                    _nativeLibraryManager?.Dispose();
                    _nativeLibraryManager = null;

                    throw;
                }
            }

            internal static void LoadPureUnsafeMethods()
            {
                _mul = PureUnsafeMethods.ReedSolomon8_Mul;
            }

            private static void GenerateGF()
            {
                string primPoly = _prim_polys[_gfBits];
                byte mask = (byte)1; // x ** 0 = 1
                _gf_exp[_gfBits] = (byte)0; // will be updated at the end of the 1st loop

                /*
                 * first, generate the (polynomial representation of) powers of \alpha,
                 * which are stored in gf_exp[i] = \alpha ** i .
                 * At the same time build gf_log[gf_exp[i]] = i .
                 * The first gfBits powers are simply bits shifted to the left.
                 */
                for (int i = 0; i < _gfBits; i++, mask <<= 1)
                {
                    _gf_exp[i] = mask;
                    _gf_log[_gf_exp[i]] = i;

                    /*
                     * If primPoly[i] == 1 then \alpha ** i occurs in poly-repr
                     * gf_exp[gfBits] = \alpha ** gfBits
                     */
                    if (primPoly[i] == '1')
                    {
                        _gf_exp[_gfBits] ^= mask;
                    }
                }

                /*
                 * now gf_exp[gfBits] = \alpha ** gfBits is complete, so can als
                 * compute its inverse.
                 */
                _gf_log[_gf_exp[_gfBits]] = _gfBits;

                /*
                 * Poly-repr of \alpha ** (i+1) is given by poly-repr of
                 * \alpha ** i shifted left one-bit and accounting for any
                 * \alpha ** gfBits term that may occur when poly-repr of
                 * \alpha ** i is shifted.
                 */
                mask = (byte)(1 << (_gfBits - 1));

                for (int i = _gfBits + 1; i < _gfSize; i++)
                {
                    if (_gf_exp[i - 1] >= mask)
                    {
                        _gf_exp[i] = (byte)(_gf_exp[_gfBits] ^ ((_gf_exp[i - 1] ^ mask) << 1));
                    }
                    else
                    {
                        _gf_exp[i] = (byte)(_gf_exp[i - 1] << 1);
                    }
                    _gf_log[_gf_exp[i]] = i;
                }

                /*
                 * log(0) is not defined, so use a special value
                 */
                _gf_log[0] = _gfSize;

                // set the extended gf_exp values for fast multiply
                for (int i = 0; i < _gfSize; i++)
                {
                    _gf_exp[i + _gfSize] = _gf_exp[i];
                }

                /*
                 * again special cases. 0 has no inverse. This used to
                 * be initialized to gfSize, but it should make no difference
                 * since noone is supposed to read from here.
                 */
                _inverse[0] = (byte)0;
                _inverse[1] = (byte)1;

                for (int i = 2; i <= _gfSize; i++)
                {
                    _inverse[i] = _gf_exp[_gfSize - _gf_log[i]];
                }
            }

            private static void InitMulTable()
            {
                _gf_mul_table = new byte[_gfSize + 1][];

                for (int i = 0; i < _gfSize + 1; i++)
                {
                    _gf_mul_table[i] = new byte[_gfSize + 1];
                }

                for (int i = 0; i < _gfSize + 1; i++)
                {
                    for (int j = 0; j < _gfSize + 1; j++)
                    {
                        _gf_mul_table[i][j] = _gf_exp[Modnn(_gf_log[i] + _gf_log[j])];
                    }
                }

                for (int i = 0; i < _gfSize + 1; i++)
                {
                    _gf_mul_table[0][i] = (byte)0;
                    _gf_mul_table[i][0] = (byte)0;
                }
            }

            private static byte Modnn(int x)
            {
                while (x >= _gfSize)
                {
                    x -= _gfSize;
                    x = (x >> _gfBits) + (x & _gfSize);
                }

                return (byte)x;
            }

            private static byte Mul(byte x, byte y)
            {
                return _gf_mul_table[x][y];
            }

            private static byte[] CreateGFMatrix(int rows, int cols)
            {
                return new byte[rows * cols];
            }

            private static void MatMul(byte[] a, int aStart, byte[] b, int bStart, byte[] c, int cStart, int n, int k, int m)
            {
                for (int row = 0; row < n; row++)
                {
                    for (int col = 0; col < m; col++)
                    {
                        int posA = row * k;
                        int posB = col;
                        byte acc = (byte)0;

                        for (int i = 0; i < k; i++, posA++, posB += m)
                        {
                            acc ^= Mul(a[aStart + posA], b[bStart + posB]);
                        }

                        c[cStart + (row * m + col)] = acc;
                    }
                }
            }

            private static void InvertMatrix(byte[] src, int k)
            {
                var indxc = new int[k];
                var indxr = new int[k];

                // ipiv marks elements already used as pivots.
                var ipiv = new int[k];

                var id_row = CreateGFMatrix(1, k);
                //byte[] temp_row = CreateGFMatrix(1, k);

                for (int col = 0; col < k; col++)
                {
                    /*
                     * Zeroing column 'col', look for a non-zero element.
                     * First try on the diagonal, if it fails, look elsewhere.
                     */
                    int irow = -1;
                    int icol = -1;
                    bool foundPiv = false;

                    if (ipiv[col] != 1 && src[col * k + col] != 0)
                    {
                        irow = col;
                        icol = col;
                        foundPiv = true;
                    }

                    if (!foundPiv)
                    {
                    loop1:
                        for (int row = 0; row < k; row++)
                        {
                            if (ipiv[row] != 1)
                            {
                                for (int ix = 0; ix < k; ix++)
                                {
                                    if (ipiv[ix] == 0)
                                    {
                                        if (src[row * k + ix] != 0)
                                        {
                                            irow = row;
                                            icol = ix;
                                            foundPiv = true;
                                            goto loop1;
                                        }
                                    }
                                    else if (ipiv[ix] > 1)
                                    {
                                        throw new ArgumentException("singular matrix");
                                    }
                                }
                            }
                        }
                    }

                    // redundant??? I'm too lazy to figure it out -Justin
                    if (!foundPiv && icol == -1)
                    {
                        throw new ArgumentException("XXX pivot not found!");
                    }

                    ipiv[icol] = ipiv[icol] + 1;

                    /*
                     * swap rows irow and icol, so afterwards the diagonal
                     * element will be correct. Rarely done, not worth
                     * optimizing.
                     */
                    if (irow != icol)
                    {
                        for (int ix = 0; ix < k; ix++)
                        {
                            // swap 'em
                            byte tmp = src[irow * k + ix];
                            src[irow * k + ix] = src[icol * k + ix];
                            src[icol * k + ix] = tmp;
                        }
                    }

                    indxr[col] = irow;
                    indxc[col] = icol;

                    int pivotRowPos = icol * k;
                    byte c = src[pivotRowPos + icol];

                    if (c == 0)
                    {
                        throw new ArgumentException("singular matrix 2");
                    }

                    if (c != 1)
                    {
                        /* otherwhise this is a NOP */
                        /*
                         * this is done often , but optimizing is not so
                         * fruitful, at least in the obvious ways (unrolling)
                         */
                        c = _inverse[c];
                        src[pivotRowPos + icol] = (byte)1;

                        for (int ix = 0; ix < k; ix++)
                        {
                            src[pivotRowPos + ix] = Mul(c, src[pivotRowPos + ix]);
                        }
                    }

                    /*
                     * from all rows, remove multiples of the selected row
                     * to zero the relevant entry (in fact, the entry is not zero
                     * because we know it must be zero).
                     * (Here, if we know that the pivotRowPos is the identity,
                     * we can optimize the addMul).
                     */
                    id_row[icol] = (byte)1;

                    if (!BytesOperations.SequenceEqual(src.AsSpan(pivotRowPos), id_row.AsSpan(), k))
                    {
                        for (int p = 0, ix = 0; ix < k; ix++, p += k)
                        {
                            if (ix != icol)
                            {
                                c = src[p + icol];
                                src[p + icol] = (byte)0;
                                AddMul(src.AsSpan(pivotRowPos), src.AsSpan(p), c, k);
                            }
                        }
                    }

                    id_row[icol] = (byte)0;
                } // done all columns

                for (int col = k - 1; col >= 0; col--)
                {
                    if (indxr[col] < 0 || indxr[col] >= k)
                    {
                        //System.err.println("AARGH, indxr[col] "+indxr[col]);
                    }
                    else if (indxc[col] < 0 || indxc[col] >= k)
                    {
                        //System.err.println("AARGH, indxc[col] "+indxc[col]);
                    }
                    else
                    {
                        if (indxr[col] != indxc[col])
                        {
                            for (int row = 0; row < k; row++)
                            {
                                // swap 'em
                                byte tmp = src[row * k + indxc[col]];
                                src[row * k + indxc[col]] = src[row * k + indxr[col]];
                                src[row * k + indxr[col]] = tmp;
                            }
                        }
                    }
                }
            }

            private static void InvertVandermonde(byte[] src, int k)
            {
                if (k == 1)
                {
                    // degenerate case, matrix must be p^0 = 1
                    return;
                }

                /*
                 * c holds the coefficient of P(x) = Prod (x - p_i), i=0..k-1
                 * b holds the coefficient for the matrix inversion
                 */
                var c = CreateGFMatrix(1, k);
                var b = CreateGFMatrix(1, k);
                var p = CreateGFMatrix(1, k);

                for (int j = 1, i = 0; i < k; i++, j += k)
                {
                    c[i] = (byte)0;
                    p[i] = src[j];    /* p[i] */
                }

                /*
                 * construct coeffs. recursively. We know c[k] = 1 (implicit)
                 * and start P_0 = x - p_0, then at each stage multiply by
                 * x - p_i generating P_i = x P_{i-1} - p_i P_{i-1}
                 * After k steps we are done.
                 */
                c[k - 1] = p[0]; /* really -p(0), but x = -x in GF(2^m) */

                for (int i = 1; i < k; i++)
                {
                    byte p_i = p[i]; /* see above comment */

                    for (int j = k - 1 - (i - 1); j < k - 1; j++)
                    {
                        c[j] ^= Mul(p_i, c[j + 1]);
                    }

                    c[k - 1] ^= p_i;
                }

                for (int row = 0; row < k; row++)
                {
                    /*
                     * synthetic division etc.
                     */
                    byte xx = p[row];
                    byte t = (byte)1;
                    b[k - 1] = (byte)1; /* this is in fact c[k] */

                    for (int i = k - 2; i >= 0; i--)
                    {
                        b[i] = (byte)(c[i + 1] ^ Mul(xx, b[i + 1]));
                        t = (byte)(Mul(xx, t) ^ b[i]);
                    }

                    for (int col = 0; col < k; col++)
                    {
                        src[col * k + row] = Mul(_inverse[t], b[col]);
                    }
                }
            }

            [HandleProcessCorruptedStateExceptions]
            public static void AddMul(ReadOnlySpan<byte> src, Span<byte> dst, byte c, int len)
            {
                // nop, optimize
                if (c == 0) return;

                // use our multiplication table.
                // Instead of doing gf_mul_table[c,x] for multiply, we'll save
                // the gf_mul_table[c] to a local variable since it is going to
                // be used many times.
                var table = _gf_mul_table[c];

                fixed (byte* p_dst = dst)
                fixed (byte* p_src = src)
                fixed (byte* p_table = table)
                {
                    _mul(p_src, p_dst, p_table, len);
                }
            }

            public static byte[] CreateEncodeMatrix(int k, int n)
            {
                if (k > _gfSize + 1 || n > _gfSize + 1 || k > n)
                {
                    throw new ArgumentException("Invalid parameters n=" + n + ",k=" + k + ",gfSize=" + _gfSize);
                }

                var encMatrix = CreateGFMatrix(n, k);

                /*
                 * The encoding matrix is computed starting with a Vandermonde matrix,
                 * and then transforming it into a systematic matrix.
                 *
                 * fill the matrix with powers of field elements, starting from 0.
                 * The first row is special, cannot be computed with exp. table.
                 */
                var tmpMatrix = CreateGFMatrix(n, k);

                tmpMatrix[0] = (byte)1;

                // first row should be 0's, fill in the rest.
                for (int pos = k, row = 0; row < n - 1; row++, pos += k)
                {
                    for (int col = 0; col < k; col++)
                    {
                        tmpMatrix[pos + col] = _gf_exp[Modnn(row * col)];
                    }
                }

                /*
                 * quick code to build systematic matrix: invert the top
                 * k*k vandermonde matrix, multiply right the bottom n-k rows
                 * by the inverse, and construct the identity matrix at the top.
                 */
                // much faster than invertMatrix
                InvertVandermonde(tmpMatrix, k);
                MatMul(tmpMatrix, k * k, tmpMatrix, 0, encMatrix, k * k, n - k, k, k);

                /*
                 * the upper matrix is I so do not bother with a slow multiply
                 */
                BytesOperations.Zero(encMatrix.AsSpan(0, k * k));

                for (int i = 0, col = 0; col < k; col++, i += k + 1)
                {
                    encMatrix[i] = (byte)1;
                }

                return encMatrix;
            }

            public static byte[] CreateDecodeMatrix(byte[] encMatrix, int[] index, int k, int n)
            {
                var matrix = CreateGFMatrix(k, k);

                for (int i = 0, pos = 0; i < k; i++, pos += k)
                {
                    BytesOperations.Copy(encMatrix.AsSpan(index[i] * k), matrix.AsSpan(pos), k);
                }

                InvertMatrix(matrix, k);

                return matrix;
            }
        }
    }
}
