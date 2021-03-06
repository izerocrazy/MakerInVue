#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)

using System;

namespace ElephantGames.Org.BouncyCastle.Math.Field
{
    public interface IPolynomial
    {
        int Degree { get; }

        //BigInteger[] GetCoefficients();

        int[] GetExponentsPresent();

        //Term[] GetNonZeroTerms();
    }
}

#endif
