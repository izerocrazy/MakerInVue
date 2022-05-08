using System;
using UnityEngine;

public static class Utility
{
	public static GameObject Find (string szPath)
	{
		Log.Asset (szPath != null);

		GameObject ret = null;
		string[] szObjectName = szPath.Split (new char[]{'.'});
		foreach (string szName in szObjectName) {
			if (ret == null) {
				ret = UnityEngine.GameObject.Find (szName);
			} else {
				ret = ret.transform.Find (szName).gameObject;
			}

			Log.Asset (ret != null);
		}

		return ret;
	}

	public static System.Object NewImplementClass (Type implementType, string szTemplateClass)
	{
		Log.Asset (szTemplateClass != null);
		Log.Asset (implementType != null);

		System.Object ret = null;

		Type templateType = Type.GetType (szTemplateClass);
		Log.Asset (templateType != null);
		Type[] templateTypeSet = new [] {templateType};

		Type resultType = implementType.MakeGenericType (templateTypeSet);
		ret = Activator.CreateInstance (resultType);

		return ret;
	}

	public static System.Object NewImplementClass (string szImplementType, params string[] arr)
	{
		Log.Asset (szImplementType != null);
		Log.Asset (arr.Length != 0);

		System.Object ret = null;

		string szTemplateList = string.Join (",", arr);
		string szResultType = string.Format ("{0}`{1}[{2}]", szImplementType, arr.Length, szTemplateList);
		Log.Info (szResultType);

		Type resultType = Type.GetType (szResultType);
		Log.Asset (resultType != null);
		ret = Activator.CreateInstance (resultType);

		return ret;
	}
}
