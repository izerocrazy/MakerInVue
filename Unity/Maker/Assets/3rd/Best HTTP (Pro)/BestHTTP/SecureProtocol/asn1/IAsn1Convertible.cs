#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
namespace ElephantGames.Org.BouncyCastle.Asn1
{
	public interface IAsn1Convertible
	{
		Asn1Object ToAsn1Object();
	}
}

#endif
