using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core;
using Omnius.Core.Serialization.RocketPack;

namespace Omnius.Core.Data
{
    public interface IOmniDatabaseFactory
    {
        ValueTask<IOmniDatabase> CreateAsync(string configPath, IBytesPool bytesPool);
    }

    public interface IOmniDatabase : IAsyncDisposable
    {
        public static IOmniDatabaseFactory Factory { get; }

        IAsyncEnumerable<string> GetKeysAsync(CancellationToken cancellationToken = default);
        ValueTask DeleteKeyAsync(string key, CancellationToken cancellationToken = default);

        ValueTask<T> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : IRocketPackObject<T>;
        ValueTask SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : IRocketPackObject<T>;
    }
}
