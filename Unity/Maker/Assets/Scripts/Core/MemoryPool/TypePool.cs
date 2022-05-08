using System.Collections;
using System.Collections.Generic;
using System;

// 类型对象缓存池
public class TypePool<T> : IPool
	where T:new()
{
	private readonly Stack<T> m_Stack = null;
	private Action<T> m_ActionOnGet;
	private Action<T> m_ActionOnRelease;

	public TypePool ()
	{
		Log.Info ("TypePool new");
		if (m_Stack == null)
			m_Stack = new Stack<T>();
	}

	public void SetAction (Action<T> actionOnGet = null, Action<T> actionOnRelease = null)
	{
		m_ActionOnGet = actionOnGet;
		m_ActionOnRelease = actionOnRelease;
	}

	public void Clear ()
	{
		m_Stack.Clear ();
		m_ActionOnGet = null;
		m_ActionOnRelease = null;
	}

	public System.Object Get ()
	{
		Log.Asset (m_Stack != null);
		Log.Info (string.Format ("TypePool {0} Get One", typeof(T)));

		T element;

		if (m_Stack.Count == 0)
		{
			element = new T();
		}
		else
		{
			element = m_Stack.Pop();
		}

		if (m_ActionOnGet != null)
			m_ActionOnGet(element);

		return element;
	}

	public void Release (System.Object element)
	{
		Log.Asset (element != null);
		Log.Asset (m_Stack != null);
		Log.Asset (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element));

		if (m_ActionOnRelease != null)
			m_ActionOnRelease ((T)element);

		m_Stack.Push ((T)element);
	}
}

public class initPoolCodeForAOT_noCall
{
	public initPoolCodeForAOT_noCall ()
	{
		// Test
		TypePool<System.Int32> Test;

		// TypePool<string> TestString;
	}
}