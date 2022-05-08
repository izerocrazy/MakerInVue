using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IModule
{
    void Init();
    void Uninit();
    void Update();
}
