using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omnix.Collections
{
    // TODO 実装中
    partial class GolombCodedSet
    {
        public async void Test()
        {
            var random = new Random();

            //this.Test2(10, 100);
            //this.Test2(100, 1000);
            this.Test2(1000000, 10000);

            //for (int i = 0; i < 1024; i++)
            //{
            //    var n = random.Next(1, 1000000);
            //    var p = random.Next(1, 1000000);

            //    this.Test2(n, p);
            //}
        }

        private void Test2(int n, int p)
        {
            var pipe = new Pipe();

            var t = new GolombEncoder(pipe.Writer, p);

            var list_1 = new List<uint>();
            {
                var random = new Random();

                for (int i = 0; i < n; i++)
                {
                    list_1.Add((uint)(random.Next() % (n * p)));
                }

                list_1.Sort();

                uint previous = 0;

                for (int i = 0; i < n; i++)
                {
                    t.Encoding(list_1[i] - previous);
                    previous = list_1[i];
                }
            }

            t.Flush();
            pipe.Writer.Complete();

            pipe.Reader.TryRead(out var readResult);
            Debug.WriteLine(readResult.Buffer.Length);

            var d = new GolombDecoder(readResult.Buffer, p);

            var list_2 = new List<uint>();

            {
                uint current = 0;

                for (; ; )
                {
                    if (!d.TryDecode(out var result)) break;
                    current += result;
                    list_2.Add(current);
                }
            }

            Debug.Assert(Enumerable.SequenceEqual(list_1, list_2));
        }

        private void Test(int n, int p)
        {
            var pipe = new Pipe();

            var t = new GolombEncoder(pipe.Writer, p);

            {
                var list = new List<uint>();
                var random = new Random();

                for (int i = 0; i < n; i++)
                {
                    list.Add((uint)(random.Next() % (n * p)));
                }

                list.Sort();

                for (int i = 1; i < n; i++)
                {
                    t.Encoding(list[i] - list[i - 1]);
                }
            }

            t.Flush();
            pipe.Writer.Complete();

            var s = pipe.Reader.ReadAsync().Result.Buffer.ToArray();

            var sb = new StringBuilder();

            foreach (var b in s)
            {
                sb.Append(Convert.ToString(b, 2));
            }

            Debug.WriteLine(s.Length);
        }

        private static uint Bitmask(int n)
        {
            return (((uint)1 << n) - 1);
        }

        private static int FloorLog2(int v)
        {
            return sizeof(int) * 8 - LeadingZeros(v - 1);
        }

        private static int LeadingZeros(int x)
        {
            const int numIntBits = sizeof(int) * 8; //compile time constant
                                                    //do the smearing
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            //count the ones
            x -= x >> 1 & 0x55555555;
            x = (x >> 2 & 0x33333333) + (x & 0x33333333);
            x = (x >> 4) + x & 0x0f0f0f0f;
            x += x >> 8;
            x += x >> 16;
            return numIntBits - (x & 0x0000003f); //subtract # of 1s from 32
        }
    }
}
