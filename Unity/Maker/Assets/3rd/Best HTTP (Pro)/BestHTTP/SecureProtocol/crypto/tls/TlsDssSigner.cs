#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

using ElephantGames.Org.BouncyCastle.Crypto.Parameters;
using ElephantGames.Org.BouncyCastle.Crypto.Signers;

namespace ElephantGames.Org.BouncyCastle.Crypto.Tls
{
    public class TlsDssSigner
        :   TlsDsaSigner
    {
        public override bool IsValidPublicKey(AsymmetricKeyParameter publicKey)
        {
            return publicKey is DsaPublicKeyParameters;
        }

        protected override IDsa CreateDsaImpl(byte hashAlgorithm)
        {
            return new DsaSigner(new HMacDsaKCalculator(TlsUtilities.CreateHash(hashAlgorithm)));
        }

        protected override byte SignatureAlgorithm
        {
            get { return Tls.SignatureAlgorithm.dsa; }
        }
    }
}

#endif
