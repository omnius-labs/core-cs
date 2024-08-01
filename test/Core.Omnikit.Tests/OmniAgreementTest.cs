using System.Text;
using Omnius.Core.Omnikit.Converters;
using Omnius.Core.Testkit;
using Xunit;
using Xunit.Abstractions;

namespace Omnius.Core.Omnikit;

public class OmniAgreementTest : TestBase<OmniAgreementTest>
{
    public OmniAgreementTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public void SimpleTest()
    {
        var agreement1 = new OmniAgreement(DateTime.UtcNow, OmniAgreementAlgorithmType.EcDhP256);
        var agreement2 = new OmniAgreement(DateTime.UtcNow, OmniAgreementAlgorithmType.EcDhP256);

        var publicKey1 = agreement1.GenAgreementPublicKey();
        var privateKey1 = agreement1.GenAgreementPrivateKey();
        var publicKey2 = agreement2.GenAgreementPublicKey();
        var privateKey2 = agreement2.GenAgreementPrivateKey();

        var secret1 = OmniAgreement.GenSecret(privateKey1, publicKey2);
        var secret2 = OmniAgreement.GenSecret(privateKey2, publicKey1);

        Assert.Equal(secret1, secret2);

        this.Output.WriteLine(Encoding.UTF8.GetString(publicKey1.PublicKey));
    }

    [Fact]
    public void CompatibilityTest()
    {
        // ref. https://github.com/omnius-labs/core-rs/blob/6ac4b87f9ec6c6de4df4d8c2aa9cb9fa7568863a/modules/omnikit/src/model/omni_agreement.rs

        var base16 = new Base16();

        var publicKey1 = new OmniAgreementPublicKey
        {
            CreatedTime = DateTime.UtcNow,
            AlgorithmType = OmniAgreementAlgorithmType.EcDhP256,
            PublicKey = base16.StringToBytes("773e70d8b7809086c16b1a6f4c5993c24351e78a31e5f020b3bcaf3bae6fac1e"),
        };
        var privateKey2 = new OmniAgreementPrivateKey
        {
            CreatedTime = DateTime.UtcNow,
            AlgorithmType = OmniAgreementAlgorithmType.EcDhP256,
            SecretKey = base16.StringToBytes("1931b4cad67708e8f525823025e4d6d7e805508e48051d148f0f61fd0801e712"),
        };

        var secret = OmniAgreement.GenSecret(privateKey2, publicKey1);

        Assert.Equal("650a5990cb592d8deda6bd1dcf7205b3cf44361290c4ed4485d4910e0b3be468", base16.BytesToString(secret));

        this.Output.WriteLine(Encoding.UTF8.GetString(secret));
    }
}
