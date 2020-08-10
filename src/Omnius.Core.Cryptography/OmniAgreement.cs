using System;
using Omnius.Core.Cryptography.Internal;
using Omnius.Core.Serialization.RocketPack;

namespace Omnius.Core.Cryptography
{
    /// <summary>
    /// Arggmentクラス
    /// </summary>
    public sealed partial class OmniAgreement
    {
        public static OmniAgreement Create(OmniAgreementAlgorithmType algorithmType)
        {
            var creationTime = DateTime.UtcNow;

            if (algorithmType == OmniAgreementAlgorithmType.EcDh_P521_Sha2_256)
            {
                var (publicKey, privateKey) = EcDh_P521_Sha2_256.CreateKeys();

                return new OmniAgreement(Timestamp.FromDateTime(creationTime), algorithmType, publicKey, privateKey);
            }
            else
            {
                throw new NotSupportedException(nameof(algorithmType));
            }
        }
        /// <summary>
        /// 公開鍵
        /// </summary>
        /// <returns></returns>
        public OmniAgreementPublicKey GetOmniAgreementPublicKey()
        {
            return new OmniAgreementPublicKey(this.CreationTime, this.AlgorithmType, this.PublicKey);
        }

        /// <summary>
        /// 秘密鍵
        /// </summary>
        /// <returns></returns>
        public OmniAgreementPrivateKey GetOmniAgreementPrivateKey()
        {
            return new OmniAgreementPrivateKey(this.CreationTime, this.AlgorithmType, this.PrivateKey);
        }

        public static ReadOnlyMemory<byte> GetSecret(OmniAgreementPublicKey publicKey, OmniAgreementPrivateKey privateKey)
        {
            if (publicKey.AlgorithmType == OmniAgreementAlgorithmType.EcDh_P521_Sha2_256
                && privateKey.AlgorithmType == OmniAgreementAlgorithmType.EcDh_P521_Sha2_256)
            {
                return EcDh_P521_Sha2_256.GetSecret(publicKey.PublicKey, privateKey.PrivateKey);
            }

            return null;
        }
    }
}
