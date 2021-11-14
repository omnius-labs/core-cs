namespace Omnius.Core.Storages;

public interface IKeyValueStorageFactory
{
    IKeyValueStorage<TKey> Create<TKey>(string path, IBytesPool bytesPool)
        where TKey : notnull;
}
