using System.Collections;

namespace Omnius.Core.Collections;

/// <summary>
/// Bloom filter.
/// </summary>
/// <typeparam name="T">Item type </typeparam>
public sealed class BloomFilter<T>
{
    private readonly BitArray _hashBits;
    private readonly Func<T, long> _hashFunction;
    private readonly int _hashFunctionCount;

    /// <summary>
    /// Creates a new Bloom filter, specifying an error rate of 1/capacity, using the optimal size for the underlying data structure based on the desired capacity and error rate, as well as the optimal number of hash functions.
    /// </summary>
    /// <param name="capacity">The anticipated number of items to be added to the filter. More than this number of items can be added, but the error rate will exceed what is expected.</param>
    /// <param name="hashFunction">The function to hash the input values. Do not use GetHashCode(). If it is null, and T is string or int a hash function will be provided for you.</param>
    public BloomFilter(int capacity, Func<T, long> hashFunction)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException("capacity", capacity, "capacity must be > 0");
        }

        var errorRate = ComputeBestErrorRate(capacity);

        _hashFunctionCount = ComputeBestK(capacity, errorRate);
        _hashBits = new BitArray(ComputeBestM(capacity, errorRate));
        _hashFunction = hashFunction;
    }

    /// <summary>
    /// Creates a new Bloom filter, using the optimal size for the underlying data structure based on the desired capacity and error rate, as well as the optimal number of hash functions.
    /// </summary>
    /// <param name="capacity">The anticipated number of items to be added to the filter. More than this number of items can be added, but the error rate will exceed what is expected.</param>
    /// <param name="errorRate">The accepable false-positive rate (e.g., 0.01F = 1%)</param>
    /// <param name="hashFunction">The function to hash the input values. Do not use GetHashCode(). If it is null, and T is string or int a hash function will be provided for you.</param>
    public BloomFilter(int capacity, double errorRate, Func<T, long> hashFunction)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException("capacity", capacity, "capacity must be > 0");
        }

        if (errorRate >= 1 || errorRate <= 0)
        {
            throw new ArgumentOutOfRangeException("errorRate", errorRate, string.Format("errorRate must be between 0 and 1, exclusive. Was {0}", errorRate));
        }

        _hashFunctionCount = ComputeBestK(capacity, errorRate);
        _hashBits = new BitArray(ComputeBestM(capacity, errorRate));
        _hashFunction = hashFunction;
    }

    /// <summary>
    /// Creates a new Bloom filter.
    /// </summary>
    public BloomFilter(int capacity, double errorRate, int hashFunctionCount, Func<T, long> hashFunction)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException("capacity", capacity, "capacity must be > 0");
        }

        if (errorRate >= 1 || errorRate <= 0)
        {
            throw new ArgumentOutOfRangeException("errorRate", errorRate, string.Format("errorRate must be between 0 and 1, exclusive. Was {0}", errorRate));
        }

        if (hashFunctionCount < 1)
        {
            throw new ArgumentOutOfRangeException(string.Format("hashFunctionCount", hashFunctionCount, "hashFunctionCount must be > 0"));
        }

        _hashFunctionCount = hashFunctionCount;
        _hashBits = new BitArray(ComputeM(capacity, errorRate, hashFunctionCount));
        _hashFunction = hashFunction;
    }

    /// <summary>
    /// The best k.
    /// </summary>
    /// <param name="capacity"> The capacity. </param>
    /// <param name="errorRate"> The error rate. </param>
    /// <returns> The <see cref="int"/>. </returns>
    private static int ComputeBestK(int capacity, double errorRate)
    {
        return (int)Math.Round(Math.Log(2.0) * ComputeBestM(capacity, errorRate) / capacity);
    }

    /// <summary>
    /// The best m.
    /// </summary>
    /// <param name="capacity"> The capacity. </param>
    /// <param name="errorRate"> The error rate. </param>
    /// <returns> The <see cref="int"/>. </returns>
    private static int ComputeBestM(int capacity, double errorRate)
    {
        return (int)Math.Ceiling(capacity * Math.Log(errorRate, (1.0 / Math.Pow(2, Math.Log(2.0)))));
    }

    private static int ComputeM(int capacity, double errorRate, int hashFunctionCount)
    {
        return (int)Math.Ceiling(capacity * (-hashFunctionCount / Math.Log(1 - Math.Exp(Math.Log(errorRate) / hashFunctionCount))));
    }

    /// <summary>
    /// The ratio of false to true bits in the filter. E.g., 1 true bit in a 10 bit filter means a truthiness of 0.1.
    /// </summary>
    public double Truthiness
    {
        get
        {
            return (double)_hashBits.GetCardinality() / _hashBits.Count;
        }
    }

    /// <summary>
    /// Adds a new item to the filter. It cannot be removed.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Add(T item)
    {
        // start flipping bits for each hash of item
        long hash = _hashFunction(item);

        for (int i = 0; i < _hashFunctionCount; i++)
        {
            int index = this.ComputeHash(hash, i);

            _hashBits[index] = true;
        }
    }

    /// <summary>
    /// Checks for the existance of the item in the filter for a given probability.
    /// </summary>
    /// <param name="item"> The item. </param>
    /// <returns> The <see cref="bool"/>. </returns>
    public bool Contains(T item)
    {
        long hash = _hashFunction(item);

        for (int i = 0; i < _hashFunctionCount; i++)
        {
            int index = this.ComputeHash(hash, i);

            if (_hashBits[index] == false)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// The best error rate.
    /// </summary>
    /// <param name="capacity"> The capacity. </param>
    /// <returns> The <see cref="double"/>. </returns>
    private static double ComputeBestErrorRate(int capacity)
    {
        double c = 1.0 / capacity;

        if (c != 0)
        {
            return c;
        }

        // default
        // http://www.cs.princeton.edu/courses/archive/spring02/cs493/lec7.pdf
        return Math.Pow(0.6185, int.MaxValue / capacity);
    }

    /// <summary>
    /// Performs Dillinger and Manolios double hashing.
    /// </summary>
    /// <returns> The <see cref="int"/>. </returns>
    private int ComputeHash(long hash, int i)
    {
        return (int)(((ulong)hash * ((ulong)i + 1)) % (ulong)_hashBits.Count);
    }
}
