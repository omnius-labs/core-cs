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
                    var pipe = new Pipe();

                    // @以降の文字列をデコードし、pipeへ書き込む。
                    OmniBase.TryDecode(item.Substring(index + 1), pipe.Writer);
                    pipe.Writer.Complete();

                    // pipeからHash情報を読み取る。
                    pipe.Reader.TryRead(out var readResult);
                    omniHash = OmniHash.Import(readResult.Buffer, BufferPool.Shared);
                    pipe.Reader.Complete();
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
                    var pipe = new Pipe();

                    // Hash情報をpipeへ書き込む。
                    this.Hash.Export(pipe.Writer, BufferPool.Shared);
                    pipe.Writer.Complete();

                    // pipeからHash情報を読み込み、Base58Btcへ変換する。
                    pipe.Reader.TryRead(out var readResult);
                    hashString = OmniBase.ToBase58BtcString(readResult.Buffer);
                    pipe.Reader.Complete();
                }

                _toString = StringHelper.Normalize(this.Name) + "@" + hashString;
            }

            return _toString;
        }
    }
}
