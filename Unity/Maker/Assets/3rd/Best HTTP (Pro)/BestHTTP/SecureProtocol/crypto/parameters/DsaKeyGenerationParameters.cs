#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

using ElephantGames.Org.BouncyCastle.Security;

namespace ElephantGames.Org.BouncyCastle.Crypto.Parameters
{
    public class DsaKeyGenerationParameters
		: KeyGenerationParameters
    {
        private readonly DsaParameters parameters;

        public DsaKeyGenerationParameters(
            SecureRandom	random,
            DsaParameters	parameters)
			: base(random, parameters.P.BitLength - 1)
        {
            this.parameters = parameters;
        }

		public DsaParameters Parameters
        {
            get { return parameters; }
        }
    }

}

#endif
