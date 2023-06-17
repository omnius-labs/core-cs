namespace Omnius.Core.Storages;

public interface IKeyValueStorageFactory
{
    IKeyValueStorage Create(string path, IBytesPool bytesPool);
}
