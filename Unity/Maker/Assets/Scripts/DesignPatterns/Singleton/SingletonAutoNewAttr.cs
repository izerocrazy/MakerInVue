using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AutoNewAttribute : Attribute
{
    public bool AutoNew;

    public AutoNewAttribute (bool bAutoNew)
    {
        AutoNew = bAutoNew;
    }
}