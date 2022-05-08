using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : SingletonObject<Main>
{
    public enum eState
    {
        None,
        Init,
        Running,
        Finish,
        Max
    }
    public eState State = eState.None;

    // Add 和 Remove 列表的设计是为了避免，某些 Module 在 Update 的过程中
    // 会告诉 GameMain 去新加或者减少 GameMian，所以在 Update 的过程中，做了缓存
    private Dictionary<string, IModule> _ModuleDict = null;
    private bool m_bProcessing = false;
    private Dictionary<string, IModule> _RemoveModules = null;
    private Dictionary<string, IModule> _AddModules = null;

    // 当前的场景
    public BaseScene CurrentScene = null;

    public bool IsConfigServerNoticeHotFix;

    public override void Init()
    {
        State = eState.Init;
        _ModuleDict = new Dictionary<string, IModule>();
        _RemoveModules = new Dictionary<string, IModule>();
        _AddModules = new Dictionary<string, IModule>();

        Log.WriteTraceLog(Log.TraceLogLevel.Info, "Game Init");
    }

    public override void Uninit()
    {
        State = eState.Finish;

        foreach (var pair in _RemoveModules)
        {
            pair.Value.Uninit();
            _ModuleDict.Remove(pair.Key);
        }
        _RemoveModules.Clear();

        foreach (var pair in _AddModules)
        {
            pair.Value.Uninit();
        }
        _AddModules.Clear();

        // LogSys 和 LuaMachine 需要单独处理
        foreach (var pair in _ModuleDict)
        {
            if (pair.Key != typeof(LogSys).ToString() && pair.Key != typeof(LuaMachine).ToString())
                pair.Value.Uninit();
        }

        if (_ModuleDict.ContainsKey(typeof(LuaMachine).ToString()))
        {
            _ModuleDict[typeof(LuaMachine).ToString()].Uninit();
        }

        // LogSys 放在最后
        if (_ModuleDict.ContainsKey(typeof(LogSys).ToString()))
        {
            _ModuleDict[typeof(LogSys).ToString()].Uninit();
        }

        _ModuleDict.Clear();
    }

    public void Update()
    {
        m_bProcessing = true;

        foreach (var pair in _ModuleDict)
        {
            if (_RemoveModules.ContainsKey(pair.Key) == false)
                pair.Value.Update();
        }

        foreach (var pair in _RemoveModules)
        {
            _ModuleDict.Remove(pair.Key);
        }
        _RemoveModules.Clear();

        // AddModule 在下一帧的时候才调用 Update
        foreach (var pair in _AddModules)
        {
            _ModuleDict.Add(pair.Key, pair.Value);
        }
        _AddModules.Clear();

        m_bProcessing = false;
    }


    #region Module
    public void AddModule(string szModuleName, IModule module)
    {
        Log.Asset(szModuleName != null && szModuleName != string.Empty, "GameMain AddModule Fail, ModuleName is Empty");
        Log.Asset(module != null, "GameMain AddModule Fail, module is Empty");

        Log.Asset(_ModuleDict != null, "GameMain AddModule Fail, you should init first");
        Log.Asset(_ModuleDict.ContainsKey(szModuleName) == false,
            string.Format("GameMain AddModule Fail, already have this Module name{0}", szModuleName));
        Log.Asset(_ModuleDict.ContainsValue(module) == false,
            string.Format("GameMain AddModule Fail, already hava this module {0}", module.ToString()));

        if (m_bProcessing == true)
        {
            _AddModules.Add(szModuleName, module);
        }
        else
        {
            _ModuleDict.Add(szModuleName, module);
        }

        Log.WriteTraceLog(Log.TraceLogLevel.Info, string.Format("GameMain AddModule, Module is {0}", szModuleName));
    }

    public bool IsHaveModule(string szNameModule)
    {
        Log.Asset(szNameModule != string.Empty && szNameModule != null, "GameMain IsHaveModule Fail, szName is Empty");
        Log.Asset(_ModuleDict != null && _RemoveModules != null && _AddModules != null, "GameMain IsHaveModule Fail, you should init first");

        return (_ModuleDict.ContainsKey(szNameModule) || _AddModules.ContainsKey(szNameModule)) && _RemoveModules.ContainsKey(szNameModule) == false;
    }

    public IModule GetModule(string szModuleName)
    {
        Log.Asset(szModuleName != string.Empty && szModuleName != null, "GameMain GetModule Fail, Module name is Empty");
        Log.Asset(_ModuleDict != null && _RemoveModules != null && _AddModules != null, "GameMain GetModule Fail, you should init first");
        Log.Asset(IsHaveModule(szModuleName),
            string.Format("GameMain GetModule Fail, don't have this Module {0}", szModuleName));

        if (_ModuleDict.ContainsKey(szModuleName))
            return _ModuleDict[szModuleName];
        else if (_AddModules.ContainsKey(szModuleName))
            return _AddModules[szModuleName];
        else
        {
            Log.Asset(false);
            return null;
        }
    }

	/*
    public LuaTable GetLuaScriptModule (string szModuleName)
    {
        LuaScriptModule luaModule = GetModule(szModuleName) as LuaScriptModule;
        return luaModule.LuaScriptTable;
    }
	*/

    public void RemoveModule(string szModuleName)
    {
        Log.Asset(_ModuleDict != null, "GameMain RemoveModule Fail, you should init first");
        Log.Asset(_RemoveModules != null, "GameMain RemoveModule Fail, you should init first");
        Log.Asset(_ModuleDict.ContainsKey(szModuleName),
            string.Format("GameMain RemoveModule Fail, this Module is not in {0}", szModuleName));

        _ModuleDict[szModuleName].Uninit();

        if (m_bProcessing)
        {
            // 不能直接在 _ModuleDict 之中删除，否则容易引发 Update 的时候 _ModuleDict 的迭代器失效
            _RemoveModules.Add(szModuleName, _ModuleDict[szModuleName]);
        }
        else
        {
            _ModuleDict.Remove(szModuleName);
        }
    }
    #endregion

    #region Singleton Module
    public T AddSingletonModule<T>()
        where T : SingletonModule<T>, new()
    {
        if (SingletonModule<T>.Instance == null)
        {
            SingletonModule<T>.CreateInstance();
        }
        else
        {
            SingletonModule<T>.Instance.Init();
        }

        T ret = SingletonModule<T>.Instance;
        Log.Asset(ret != null, string.Format("GameMain AddModule Fail, {0} GetInstance fail", typeof(T).ToString()));

        AddModule(typeof(T).ToString(), ret);

        return ret;
    }

    public bool IsHaveSingletonModule<T>()
        where T : ISingletonObject
    {
        return IsHaveModule(typeof(T).ToString());
    }

    public T GetSingletonModule<T>()
        where T : ISingletonObject
    {
        Log.Asset(IsHaveSingletonModule<T>(),
            string.Format("GameMain GetModule Fail, don't have this Module {0}", typeof(T).ToString()));

        return (T)GetModule(typeof(T).ToString());
    }

    public void RemoveSingletomModule<T>()
    {
        RemoveModule(typeof(T).ToString());
    }
    #endregion

    #region LuaScript Module
	/*
    public bool AddLuaScriptModule (string szLuaFilePath)
    {
        bool bRet = false;

        Log.Asset(szLuaFilePath != null && szLuaFilePath != string.Empty, "GameMain AddLuaScriptModule Fail, szLuaFilePath is Empty");

        LuaScriptModule module = new LuaScriptModule();
        module.Init();
        module.RunScript(szLuaFilePath);

        AddModule(szLuaFilePath, module);

        // binding Lua Machine Restart
        LuaMachine machine = GetSingletonModule<LuaMachine>();
        //machine.RegisterRestartCallback(module.ReloadScript);
        machine.RegisterBeforeRestartCallback(module.UninitLua);
        machine.RegisterAfterRestartCallback(module.ReloadLua);

        bRet = true;
        return bRet;
    }
	*/
    #endregion

    public void StartRunning(Action func)
    {
        Log.Asset(State == eState.Init, string.Format("Game Main StartRunning Fail, State is {0}", State));
        State = eState.Running;

        if (func != null)
        {
            func();
        }

        Log.WriteTraceLog(Log.TraceLogLevel.Info, "Game Start Running");

		Log.Asset (CurrentScene != null);
		CurrentScene.OnMainRunning ();
    }

    public void StopRunning(Action func)
    {
        Log.Asset(State == eState.Running, string.Format("Game Main StopRunning Fail, State is {0}", State));
        State = eState.Finish;

        if (func != null)
        {
            func();
        }

        Log.WriteTraceLog(Log.TraceLogLevel.Info, "Game Stop Running");
    }

    ///////////////////////////////////
    // Lua Helper
	/*
    public LogSys GetLogModule()
    {
        return GetSingletonModule<LogSys>();
    }

    public AnalyticsSys GetAnalyticsModule()
    {
        return GetSingletonModule<AnalyticsSys>();
    }
	*/
}



