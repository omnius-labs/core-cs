using System;
using Omnius.Core.Helpers;
using Omnius.Core.Pipelines;
using Omnius.Core.Serialization;

namespace Omnius.Core.Cryptography
{
    public sealed partial class OmniSignature
    {
        public static bool TryParse(string item, out OmniSignature? signature)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            signature = null;

            try
            {
                int index = item.IndexOf('@');
                if (index == -1) return false;

                // @より前の文字列を取得
                string name = item.Substring(0, index);

                OmniHash omniHash;

                using (var bytesPipe = new BytesPipe())
                {
                    // @以降の文字列をデコードし、bytesPipeへ書き込む。
                    OmniBase.TryDecode(item.Substring(index + 1), bytesPipe.Writer);

                    // bytesPipeからHash情報を読み取る。
                    omniHash = OmniHash.Import(bytesPipe.Reader.GetSequence(), BytesPool.Shared);
                }

                signature = new OmniSignature(name, omniHash);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string? _toString;

        public override string ToString()
        {
            if (_toString == null)
            {
                string hashString;

                using (var bytesPipe = new BytesPipe())
                {
                    // Hash情報をbytesPipeへ書き込む。
                    this.Hash.Export(bytesPipe.Writer, BytesPool.Shared);

                    // bytesPipeからHash情報を読み込み、Base58Btcへ変換する。
                    hashString = OmniBase.Encode(bytesPipe.Reader.GetSequence(), ConvertStringType.Base58) ?? string.Empty;
                }

                _toString = StringHelper.Normalize(this.Name) + "@" + hashString;
            }

            return _toString;
        }
    }
}
