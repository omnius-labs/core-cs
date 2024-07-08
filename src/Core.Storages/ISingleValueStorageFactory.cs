using Omnius.Core.Base;

namespace Omnius.Core.Storages;

public interface ISingleValueStorageFactory
{
    ISingleValueStorage Create(string path, IBytesPool bytesPool);
}
