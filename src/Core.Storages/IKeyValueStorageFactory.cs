using Core.Base;

namespace Core.Storages;

public interface IKeyValueStorageFactory
{
    IKeyValueStorage Create(string path, IBytesPool bytesPool);
}
