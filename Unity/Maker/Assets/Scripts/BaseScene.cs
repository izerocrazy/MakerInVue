//#define OPTIMIZE_TEST

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if OPTIMIZE_TEST
using UnityEngine.Profiling;
#endif

public class BaseScene : MonoBehaviour
{
    public int TargetFrame = 40;

    // Use this for initialization
    void Awake()
    {
        if (Main.Instance.State == Main.eState.Init)
        {
            GameObject.DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = TargetFrame;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Main.Instance.CurrentScene = this;
#if OPTIMIZE_TEST
            UnityEngine.Profiling.Profiler.BeginSample("BaseScene.Awake:GameMain.Instance.StartRunning");
#endif
            Main.Instance.StartRunning(() =>
				{
					#region PoolManager 模块（内存优化） 
					PoolManager pm = Main.Instance.AddSingletonModule<PoolManager>();
					#endregion

	                #region LogSys 
#if OPTIMIZE_TEST
	            	UnityEngine.Profiling.Profiler.BeginSample("BaseScene.Awake:AddModule<LogSys>");
#endif
	                LogSys sys = Main.Instance.AddSingletonModule<LogSys>();
	                // 初始化 Log1: Set Folder
	                string path = "";
	                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
	                {
	                    path = Path.Combine(Application.persistentDataPath, "Log");
	                }
	                else
	                {
	                    path = Path.GetFullPath(Path.Combine(Application.dataPath, "./../Log"));
	                }
	                sys.SetFolder(path);

	                // 初始化 Log2: Set Log Name
	                string szFileName = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
	                sys.SetLogFile(szFileName);

	                // 清理 10 天前的 Log
	                sys.ClearOldLogs(10);
#if OPTIMIZE_TEST
	            	UnityEngine.Profiling.Profiler.EndSample();
#endif
	                #endregion

	                #region LuaMachine 模块（CPU）
#if OPTIMIZE_TEST
	            	UnityEngine.Profiling.Profiler.BeginSample("BaseScene.Awake:AddModule<LuaMachine>");
#endif
	                LuaMachine machine = Main.Instance.AddSingletonModule<LuaMachine>();
#if OPTIMIZE_TEST
	            	UnityEngine.Profiling.Profiler.EndSample();
#endif
					machine.SetPathReader(( string szFile )=>{
						if (szFile.EndsWith(".lua") == false)
						{
							szFile = szFile + ".lua";
						}

						Log.Info ("Load File:" + szFile);
						return (Resources.Load(szFile) as TextAsset).text;
					});
					#endregion

					// 载入 Lua 的核心，这里应该使用 LuaScriptModule
					machine.DoFile ("LuaFile/Core/Main");

					// 理想中的 Core 到此为止，为了测试，这里添加入了 ResourceManager

            	});
#if OPTIMIZE_TEST
            UnityEngine.Profiling.Profiler.EndSample();
#endif
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Main.Instance.State == Main.eState.Running)
        {
            Main.Instance.Update();
        }
    }

    public void OnDestroy()
    {
        if (Main.Instance.State == Main.eState.Running)
        {
            Main.Instance.StopRunning(() =>
				{
					#region LuaMachine 的清理
					LuaMachine machine = Main.Instance.GetSingletonModule<LuaMachine>();
					Log.Asset(machine != null);
					#endregion

	                Main.Instance.Uninit();
	            }
			);
        }
    }

	public virtual void OnMainRunning ()
	{
		Log.Info ("BaseScene OnMainRunning");
	}
}
