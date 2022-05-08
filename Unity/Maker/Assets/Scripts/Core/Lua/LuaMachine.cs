using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XLua;

public class LuaMachine: SingletonModule<LuaMachine>
{
    private LuaEnv _LuaEnv = null;
    public delegate void RestartCallbackDelegate();
    private event Action BeforeRestartCallbacks;
    private event Action AfterRestartCallbacks;
    private event RestartCallbackDelegate restartCallbacks;
	 
	// 加载器
	public delegate string PathReader (string szFile);
	private PathReader m_funcPathReader = null;
	public void SetPathReader (PathReader func)
	{
		m_funcPathReader = func;
	}

	public void ClearPathReader ()
	{
		m_funcPathReader = null;
	}

    public LuaEnv Env
    {
        get
        {
            return _LuaEnv;
        }
    }

    public override void Init()
    {
        Log.Asset(_LuaEnv == null, "LuaMachine Init Fail, you can't init twice");
        praperaLuaEnv();
    }

    public override void Uninit()
    {
        Log.Asset(_LuaEnv != null, "LuaMachine Uninit Fail, you should init first");
        clearLuaEnv();
		ClearPathReader ();
    }

    public void Restart ()
    {
        Log.Asset(_LuaEnv != null);
        //Log.Asset(restartCallbacks != null);

        Log.Info ("LuaMachine start Restart");
        
        if (BeforeRestartCallbacks != null)
        {
            try
            {
                BeforeRestartCallbacks();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

		ClearPathReader();
        clearLuaEnv();
        praperaLuaEnv();

        Log.Info("LuaMachine Restart finish");
        if (restartCallbacks != null)
            restartCallbacks();

        if (AfterRestartCallbacks != null)
        {
            try
            {
                AfterRestartCallbacks();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }

    public void RegisterBeforeRestartCallback(Action callback)
    {
        Log.Asset(callback != null);
        if (BeforeRestartCallbacks != null && BeforeRestartCallbacks.GetInvocationList().Contains(callback))
            return;

        BeforeRestartCallbacks += callback;
    }

    public void RegisterAfterRestartCallback(Action callback)
    {
        Log.Asset(callback != null);
        if (AfterRestartCallbacks != null && AfterRestartCallbacks.GetInvocationList().Contains(callback))
            return;

        AfterRestartCallbacks += callback;
    }


    public void UnregisterBeforeRestartCallback(Action callback)
    {
        Log.Asset(callback != null);

        BeforeRestartCallbacks -= callback;
    }

    public void UnregisterAfterRestartCallback(Action callback)
    {
        Log.Asset(callback != null);

        AfterRestartCallbacks -= callback;
    }

    //public void RegisterRestartCallback (RestartCallbackDelegate func)
    //{
    //    Log.Asset(func != null);
    //    Log.Asset(restartCallbacks == null || restartCallbacks.GetInvocationList().Contains(func) == false);

    //    restartCallbacks += func;
    //}

    //public void UnregisterRestartCallback (RestartCallbackDelegate removeFunc)
    //{
    //    Log.Asset(removeFunc != null);
    //    Log.Asset(restartCallbacks.GetInvocationList().Contains(removeFunc) == true);

    //    restartCallbacks -= removeFunc;
    //}

	private string getStrFromFile (string filename)
	{
		string luaFileStr = filename;
        if (m_funcPathReader != null)
            luaFileStr = m_funcPathReader(filename);
        else
        {
            var txt = Resources.Load<TextAsset>(filename);
            if (txt != null)
                luaFileStr = txt.text;
            else
                throw new Exception("luaFile is missing:" + filename);
        }

		return luaFileStr;
	}

    private void praperaLuaEnv ()
    {
        _LuaEnv = new LuaEnv();
        _LuaEnv.AddLoader((ref string filename) =>
        {
			string luaFileStr = getStrFromFile(filename);
			if (string.IsNullOrEmpty(luaFileStr))
				Debug.LogWarning("Lua文件不存在：" + filename);
			else
			{
				byte[] readFile = System.Text.Encoding.UTF8.GetBytes(luaFileStr);
				return readFile;
			}

			return null;
        });

        _LuaEnv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
    }

    private void clearLuaEnv ()
    {
        try
        {
            /*
             * todo: 勿删除
            foreach (var func in lstDisposeCallback)
            {
                func.Call();
                func.Dispose();
            }

            lstDisposeCallback.Clear();
            */
            _LuaEnv.Dispose();
        }
        catch (Exception ex)
        {
            Log.WriteTraceLog(Log.TraceLogLevel.Error, 
                string.Format("LuaMachine Uninit Fail, Catch A Lua Exception :\n{0}", ex.ToString()));
        }
    }

    public override void Update ()
    {
        if (_LuaEnv != null)
        {
            // 不设限
            _LuaEnv.Tick();
        }
    }

    /*
    public void SetLuaFolder (string szPath)
    {
        Log.Asset(szPath != null && szPath != string.Empty);
        Log.Asset(_LuaFolderPath == null);

        _LuaFolderPath = szPath;
    }
    */


    public object [] DoFile (string fileName)
    {
        Log.Asset(fileName != null && fileName != string.Empty, "LuaMachine DoFile Fail, fileName is Empty");
        Log.Asset(_LuaEnv != null, "LuaMachine DoFile Fail, you should init first");

		string luaFileStr = getStrFromFile(fileName);

        Log.Asset(luaFileStr != null && luaFileStr != string.Empty, 
            string.Format("LuaMachine DoFile Fail, FileStr is Empty, file name is {0}", fileName));

        return _LuaEnv.DoString(luaFileStr, fileName);
    } 

    public object[] DoString (string szLuaData, string fileName = "chunk")
    {
        Log.Asset(szLuaData != null && szLuaData != string.Empty, "LuaMachine DoString Fail, LuaString is Empty");
        Log.Asset(_LuaEnv != null, "LuaMachine DoString Fail, you should init first");

        object[] luaRet = _LuaEnv.DoString(szLuaData, fileName);

        return luaRet;
    }

    public T GetLuaValue<T>(string funcName)
    {
        Log.Asset(funcName != null && funcName != string.Empty, "LuaMachine GetLuaValue Fail, funcName is Empty");
        Log.Asset(_LuaEnv != null, "LuaMachine GetLuaValue Fail, _LuaEnv GetLuaValue Fail");
        return _LuaEnv.Global.Get<T>(funcName);
    }

}
