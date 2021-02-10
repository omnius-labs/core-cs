using System;
using Omnius.Core.Helpers;
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

                using (var hub = new BytesHub())
                {
                    // @以降の文字列をデコードし、hubへ書き込む。
                    OmniBase.TryDecode(item.Substring(index + 1), hub.Writer);

                    // hubからHash情報を読み取る。
                    omniHash = OmniHash.Import(hub.Reader.GetSequence(), BytesPool.Shared);
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

                using (var hub = new BytesHub())
                {
                    // Hash情報をhubへ書き込む。
                    this.Hash.Export(hub.Writer, BytesPool.Shared);

                    // hubからHash情報を読み込み、Base58Btcへ変換する。
                    hashString = OmniBase.Encode(hub.Reader.GetSequence(), ConvertStringType.Base58);
                }

                _toString = StringHelper.Normalize(this.Name) + "@" + hashString;
            }

            return _toString;
        }
    }
}
