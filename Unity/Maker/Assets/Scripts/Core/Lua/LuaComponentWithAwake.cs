using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuaComponentWithAwake : LuaComponent {
    private void Awake()
    {
        RunLuaFile(LuaFilePath);
    }
}
