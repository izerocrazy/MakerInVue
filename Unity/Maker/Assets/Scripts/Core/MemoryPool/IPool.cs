using System;

public interface IPool
{
	System.Object Get();
	void Release (System.Object element);
	void Clear ();
}
