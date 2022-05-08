using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[AutoNew(false)]
public class Module : IModule
{
    public virtual void Init()
    {
        throw new NotImplementedException();
    }

    public virtual void Uninit()
    {
        throw new NotImplementedException();
    }

    public virtual void Update()
    {
        throw new NotImplementedException();
    }
}

