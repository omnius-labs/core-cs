using System.Security.Cryptography;
using System.Text;
using Omnius.Core.Omnikit.Converters;
using Omnius.Core.Testkit;
using Xunit;
using Xunit.Abstractions;

namespace Omnius.Core.Omnikit;

public class omniSignTest : TestBase<omniSignTest>
{
    public omniSignTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public void SimpleTest()
    {
        var signer = new OmniSigner(OmniSignType.Ed25519_Sha3_256_Base64Url, "test_user");
        var signature = signer.Sign("test"u8.ToArray());

        Console.WriteLine(signer);
        Console.WriteLine(signature);

        signature.Verify("test"u8.ToArray());
        try
        {
            signature.Verify("test_err"u8.ToArray());
        }
        catch (CryptographicException)
        {
            Console.WriteLine("Verification failed as expected");
        }
    }

    [Fact]
    public void CompatibilityTest()
    {
        // ref. https://github.com/omnius-labs/core-rs/blob/6ac4b87f9ec6c6de4df4d8c2aa9cb9fa7568863a/modules/omnikit/src/model/omni_sign.rs

        var cert = new OmniCert()
        {
            Name = "",
            Type = OmniSignType.Ed25519_Sha3_256_Base64Url,
            PublicKey = Base16.Lower.StringToBytes("302a300506032b6570032100ec03765ff80e8f72365b8d238d7894e77d6e02053e526223893dec9a51012caf"),
            Value = Base16.Lower.StringToBytes("07b7f720f2a455fecfc723e6fcb8f19a765d5b967c8e1dc4d57f6c8cfeda6bc1f6ae2765085a28a18cc9c1c622cc256e713566f9c3d8e4f2de8e756aee8f600d"),
        };
        Assert.True(cert.Verify("test"u8.ToArray()));
    }
}
