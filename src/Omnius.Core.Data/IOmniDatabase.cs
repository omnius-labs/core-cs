using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Omnius.Core;
using Omnius.Core.Serialization.RocketPack;

namespace Omnius.Core.Data
{
    public interface IOmniDatabase : IAsyncDisposable
    {
        ValueTask<T> LoadAsync<T>(string key) where T : IRocketPackObject<T>;
        ValueTask SaveAsync<T>(string key, T value) where T : IRocketPackObject<T>;
    }
}
