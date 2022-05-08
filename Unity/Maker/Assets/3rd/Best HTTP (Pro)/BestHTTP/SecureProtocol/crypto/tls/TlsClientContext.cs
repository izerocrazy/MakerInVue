#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

using ElephantGames.Org.BouncyCastle.Security;

namespace ElephantGames.Org.BouncyCastle.Crypto.Tls
{
    public interface TlsClientContext
        :   TlsContext
    {
    }
}

#endif
