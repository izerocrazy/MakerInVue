using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLua;

[AutoNew(false)]
public class LuaScriptModule : LuaScriptClassObj, IModule
{
    LuaFunction LuaInitCall = null;
    LuaFunction LuaUninitCall = null;
    LuaFunction LuaUpdateCall = null;   // 提供 Update 能力给 Lua

    public virtual void Init()
    {
        Log.Asset(LuaScriptTable == null);
    }

    public virtual void Uninit()
    {
        if (LuaScriptTable != null && LuaUninitCall != null)
        {
            LuaUninitCall.Call(LuaScriptTable);
        }
        LuaInitCall = null;
        LuaUninitCall = null;
        LuaUpdateCall = null;

        LuaFilePath = null;

    }

    public virtual void Update()
    {
        if (LuaScriptTable != null && LuaUpdateCall != null)
        {
            LuaUpdateCall.Call(LuaScriptTable);
        }
    }

    public override void RunScript (string szLuaFilePath)
    {
        base.RunScript(szLuaFilePath);
        Log.Asset(this.LuaScriptTable != null);

        // 取得常用的函数回调
        this.LuaScriptTable.Get("Init", out LuaInitCall);
        Log.Asset(LuaInitCall != null, "LuaScriptModule RunScript Fail, There should have a Init function");
        this.LuaScriptTable.Get("Update", out LuaUpdateCall);
        Log.Asset(LuaUpdateCall != null, "LuaScriptModule RunScript Fail, There should have a Update Function");
        this.LuaScriptTable.Get("Uninit", out LuaUninitCall);
        Log.Asset(LuaUninitCall != null, "LuaScriptModule RunScript Fail, There should havee a Uninit Function");

        // 调用 Init
        LuaInitCall.Call(LuaScriptTable);
        Log.Info (string.Format("LuaScriptModule RunScript, module is {0}", szLuaFilePath));

		LuaFilePath = szLuaFilePath;
    }

    public virtual void UninitLua()
    {
        Log.Info(string.Format("LuaScriptModule {0} UninitLua", LuaFilePath));
        if (LuaUninitCall != null)
            LuaUninitCall.Call(LuaScriptTable);
    }
    
    public virtual void ReloadLua()
    {
        Log.Info(string.Format("LuaScriptModule {0} ReloadLua", LuaFilePath));

        LuaInitCall = null;
        LuaUninitCall = null;
        LuaUpdateCall = null;

        string oldPath = LuaFilePath;
        LuaFilePath = null;

        RunScript(oldPath);
    }

    public virtual void ReloadScript ()
    {
		Log.Info (string.Format("LuaScriptModule {0} ReloadScript", LuaFilePath));

        LuaInitCall = null;
        LuaUninitCall = null;
        LuaUpdateCall = null;

		string oldPath = LuaFilePath;
        LuaFilePath = null;

        RunScript(oldPath);
    }
}

