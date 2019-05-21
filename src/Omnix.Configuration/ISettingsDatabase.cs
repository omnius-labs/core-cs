using System;
using Omnix.Serialization.RocketPack;

namespace Omnix.Configuration
{
    public interface ISettingsDatabase : IDisposable
    {
        int GetVersion();
        void SetVersion(int version);

        bool TryGetContent<T>(string name, out T value) where T : RocketPackMessageBase<T>;
        bool TryGetContent<T>(string name, out T value, IRocketPackFormatter<T> formatter);

        void SetContent<T>(string name, T value) where T : RocketPackMessageBase<T>;
        void SetContent<T>(string name, T value, IRocketPackFormatter<T> formatter);

        void Commit();
        void Rollback();
    }
}
