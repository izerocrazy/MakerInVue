#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

namespace ElephantGames.Org.BouncyCastle.Crypto.Tls
{
    public interface TlsHandshakeHash
        :   IDigest
    {
        void Init(TlsContext context);

        TlsHandshakeHash NotifyPrfDetermined();

        void TrackHashAlgorithm(byte hashAlgorithm);

        void SealHashAlgorithms();

        TlsHandshakeHash StopTracking();

        IDigest ForkPrfHash();

        byte[] GetFinalHash(byte hashAlgorithm);
    }
}

#endif
