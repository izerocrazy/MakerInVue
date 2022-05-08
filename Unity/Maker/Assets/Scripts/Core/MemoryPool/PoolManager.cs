using System.Collections;
using System.Collections.Generic;
using System;

public class PoolManager:SingletonModule<PoolManager> 
{
	public Dictionary<string, IPool> m_Pools = null;

	public override void Init ()
	{
		Log.Asset (m_Pools == null);

		m_Pools = new Dictionary<string, IPool> ();
	}

	public override void Uninit ()
	{
		Log.Asset (m_Pools != null);
		foreach (var pair in m_Pools) {
			pair.Value.Clear ();
		}

		m_Pools.Clear ();
	}

	public override void Update ()
	{
	}

	public void AddPool (string szType)
	{
		Log.Asset (szType != null);
		Log.Asset (m_Pools != null);

		System.Object o = Utility.NewImplementClass ("TypePool", szType);
		m_Pools.Add (szType, o as IPool);
	}

	public System.Object GetObject (string szType)
	{
		return m_Pools [szType].Get ();
	}
}
