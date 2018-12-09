using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.Serialization;
using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Serialization;

namespace Omnix.Cryptography
{
    public sealed partial class OmniSignature
    {
        public static OmniSignature Parse(string item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            try
            {
                int index = item.IndexOf('@');
                if (index == -1) return null;

                // @より前の文字列を取得
                string name = item.Substring(0, index);

                OmniHash omniHash;
                {
                    var hub = new Hub();

                    // @以降の文字列をデコードし、hubへ書き込む。
                    OmniBase.TryDecode(item.Substring(index + 1), hub.Writer);
                    hub.Writer.Complete();

                    // hubからHash情報を読み取る。
                    omniHash = OmniHash.Import(hub.Reader.GetSequence(), BufferPool.Shared);
                    hub.Reader.Complete();

                    hub.Reset();
                }

                return new OmniSignature(name, omniHash);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private volatile string _toString;

        public override string ToString()
        {
            if (_toString == null)
            {
                string hashString;
                {
                    var hub = new Hub();

                    // Hash情報をhubへ書き込む。
                    this.Hash.Export(hub.Writer, BufferPool.Shared);
                    hub.Writer.Complete();

                    // hubからHash情報を読み込み、Base58Btcへ変換する。
                    hashString = OmniBase.ToBase58BtcString(hub.Reader.GetSequence());
                    hub.Reader.Complete();

                    hub.Reset();
                }

                _toString = StringHelper.Normalize(this.Name) + "@" + hashString;
            }

            return _toString;
        }
    }
}
