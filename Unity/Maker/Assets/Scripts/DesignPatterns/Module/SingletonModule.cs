using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[AutoNew(false)]
public class SingletonModule<T> : SingletonObject<T>, IModule
    where T : ISingletonObject, new()
{
    public virtual void Update()
    {
        throw new NotImplementedException();
    }
}
