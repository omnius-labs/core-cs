using System.Diagnostics.CodeAnalysis;
using Omnius.Core.Helpers;
using Omnius.Core.Pipelines;
using Omnius.Core.Serialization;

namespace Omnius.Core.Cryptography;

public sealed partial class OmniSignature
{
    public static OmniSignature Parse(string item)
    {
        if (!TryParse(item, out var signature)) throw new FormatException();
        return signature;
    }

    public static bool TryParse(string item, [NotNullWhen(true)] out OmniSignature? signature)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        signature = null;

        try
        {
            int index = item.IndexOf('@');
            if (index == -1) return false;

            // @より前の文字列を取得
            string name = item[..index];

            OmniHash omniHash;

            using (var bytesPipe = new BytesPipe())
            {
                // @以降の文字列をデコードし、bytesPipeへ書き込む。
                OmniBase.TryDecode(item[(index + 1)..], bytesPipe.Writer);

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
                hashString = OmniBase.Encode(bytesPipe.Reader.GetSequence(), ConvertBaseType.Base58Btc) ?? string.Empty;
            }

            _toString = StringHelper.Normalize(this.Name) + "@" + hashString;
        }

        return _toString;
    }
}
