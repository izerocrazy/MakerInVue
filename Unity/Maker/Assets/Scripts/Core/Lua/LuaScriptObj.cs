using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLua;

public class LuaScriptObj
{
    public string LuaFilePath = null; 
    public LuaTable LuaScriptTable
    {
        get;
        protected set;
    }

    public virtual void RunScript (string szLuaFilePath)
    {
        Log.Asset(LuaFilePath == null, "LuaScriptObj RunScript Fail, Already Have A Script");
		Log.Asset(szLuaFilePath != null && szLuaFilePath != string.Empty, "LuaScriptObj RunScript Fail, szLuaFilePath is Empty");

        LuaMachine machine = Main.Instance.GetSingletonModule<LuaMachine>();
        Log.Asset(machine != null && machine.Env != null, "LuaScriptObj RunScript Fail, LuaMachine is Empty");
        object[] luaRet = machine.DoFile(szLuaFilePath);
        Log.Asset(luaRet != null && luaRet.Length >= 1);

        LuaFilePath = szLuaFilePath;
        
        // 约定：第一个返回的 Table 对象作为 LuaScriptTable
        LuaScriptTable = luaRet[0] as LuaTable;
    }

    public virtual object[] CallFunc (string szFuncName, params object[] args)
    {
        Log.Asset(szFuncName != null && szFuncName != string.Empty);
        
        LuaFunction func = null;
        LuaScriptTable.Get(szFuncName, out func);
        Log.Asset(func != null);

        return func.Call(args);
    }

    public virtual object this[string szField]
    {
        get
        {
            return LuaScriptTable.GetInPath<object>(szField);
        }
        set
        {
            LuaScriptTable.SetInPath<object>(szField, value);
        }
    }
}

public class LuaScriptClassObj:LuaScriptObj
{
    public override void RunScript (string szLuaFilePath)
    {
        Log.Asset(LuaFilePath == null, "LuaScriptClassObj RunScript Fail, Already Have A Script");
		Log.Asset(szLuaFilePath != null && szLuaFilePath != string.Empty, "LuaScriptClassObj RunScript Fail, szLuaFilePath is Empty");

        LuaMachine machine = Main.Instance.GetSingletonModule<LuaMachine>();
        Log.Asset(machine != null && machine.Env != null, "LuaScriptClassObj RunScript Fail, LuaMachine is Empty");
        object[] luaRet = machine.DoFile(szLuaFilePath);
        Log.Asset(luaRet != null && luaRet.Length >= 1);
        
        LuaFilePath = szLuaFilePath;

        // 约定：第一个返回的Table对象作为Lua模块的 Class
        LuaTable moduleClass = luaRet[0] as LuaTable;
        LuaFunction luaNewClassFunc;
        moduleClass.Get("new", out luaNewClassFunc);
        Log.Asset(luaNewClassFunc != null);
        LuaScriptTable = luaNewClassFunc.Call()[0] as LuaTable;
    }
    

    public bool InitFromLuaClass(LuaTable classTable)
    {
        Log.Asset(classTable != null, "LuaScriptClassObj InitFromLuaClass fail, classTable is null.");
        Log.Asset(LuaScriptTable == null, "LuaScriptClassObj InitFromLuaClass fail, already has a LuaScriptTable.");

        LuaFilePath = null;
        LuaFunction luaNewClassFunc = classTable.Get<LuaFunction>("new");
        Log.Asset(luaNewClassFunc != null);
        LuaScriptTable = luaNewClassFunc.Call()[0] as LuaTable;

        return true;
    }

    public virtual object[] CallSelfFunc (string szFuncName, params object[] args)
    {
        Log.Asset(szFuncName != null && szFuncName != string.Empty, "LuaScriptObj CallSelfFunc Fail, szFuncName is Empty");
        
        LuaFunction func = null;
        LuaScriptTable.Get(szFuncName, out func);
        Log.Asset(func != null, string.Format("LuaScriptObj CallSelfFunc Fail, Cant find func {1}:{0}", szFuncName, LuaFilePath));

        Array.Resize(ref args, args.Length + 1);
        for (int i = args.Length - 1; i >= 1; i--)
            args[i] = args[i-1];
        args[0] = LuaScriptTable;

        //List<object> lstParam = args.ToList();
        //lstParam.Insert(0, LuaScriptTable);

        return func.Call(args);
    }

    public virtual object[] CallSelfFunc(string szFuncName)
    {
        Log.Asset(szFuncName != null && szFuncName != string.Empty, "LuaScriptObj CallSelfFunc Fail, szFuncName is Empty");

        LuaFunction func = null;
        LuaScriptTable.Get(szFuncName, out func);
        Log.Asset(func != null, string.Format("LuaScriptObj CallSelfFunc Fail, Cant find func {1}:{0}", szFuncName, LuaFilePath));

        return func.Call(LuaScriptTable);
    }
}