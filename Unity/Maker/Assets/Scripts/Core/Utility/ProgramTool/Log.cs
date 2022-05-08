 // #define __DEBUG_

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Log:SingletonObject<Log>
{
    public bool Close = false;
    private Dictionary<TraceLogLevel, Action<string>> CallbackDict = null;

    public enum TraceLogLevel
    {
        None = 0,
        Debug,
        Info,
        Warning,
        Error,
        Max
    }

    public override void Init ()
    {
        // 不要在此处使用 Log.Asset，因此还没有准备好回调函数
        if (CallbackDict == null)
        {
            CallbackDict = new Dictionary<TraceLogLevel, Action<string>>();
        }
    }

    public override void Uninit()
    {
        Log.Asset(CallbackDict != null, "Log.Uninit Fail, you should init first");
        CallbackDict.Clear();

        return;
    }

    public void SetLevelCallback (TraceLogLevel level, Action<string> func)
    {
        // 不要在此处使用 Log.Asset，因为此时正在准备回调函数
        if (level <= TraceLogLevel.None && level >= TraceLogLevel.Max)
        {
            throw new Exception(string.Format("Log.SetLevelCallback Fail, Level is Error {0}", level));
        }

        if (func == null)
        {
            throw new Exception("Log.SetLevelCallback Fail, func is Empty");
        }

        if (CallbackDict == null)
        {
            throw new Exception("Log.SetLevelCallback Fail, you should init first");
        }

        if (CallbackDict.Keys.Contains(level))
        {
            throw new Exception("Log.SetLevelCallback Fail, you already Set this callback");
        }

        CallbackDict[level] = func;
    }

    public void ClearLevelCallback (TraceLogLevel level)
    {
        Log.Asset(level > TraceLogLevel.None && level < TraceLogLevel.Max,
            string.Format("Log.ClearLevelCallback Fail, Level is Error {0}", level));

        Log.Asset(CallbackDict != null,
            string.Format("Log.ClearLevelCallback Fail, you should init first"));

        Log.Asset(CallbackDict[level] != null,
            string.Format("Log.ClearLevelCallback Fail, there is not call in this level {0}", level));

        CallbackDict.Remove(level);
    }

	public static void Asset (System.Object obj, string szLog)
	{
		Asset (obj != null, szLog);
	}

    public static void Asset (bool bValue, string szLog = "Get A Asset Fail")
    {
        if (Log.Instance.Close)
            return;

        if (szLog == null || szLog == "" || szLog == string.Empty)
        {
            throw new Exception("Log Asset Get A Empty String");
        }

        if (bValue == false)
        {
            Log.WriteTraceLog(Log.TraceLogLevel.Error, szLog);
            throw new Exception(szLog);
        }
    }

    public static void AssetFormat (bool bValue, string szLog = "Get A Asset Fail", params object[] lstParams)
    {
        Asset(bValue, string.Format(szLog, lstParams));
    }

    public static void WriteTraceLog (TraceLogLevel level, string szLog)
    {
        if (Log.Instance.Close)
            return;

        Log.Asset(level > TraceLogLevel.None && level < TraceLogLevel.Max,
            string.Format("Log.WriteTraceLog Fail, Level is Error {0}", level));

        Log.Asset(szLog != null && szLog != "" && szLog != string.Empty, 
            string.Format("Log.WriteTraceLog Fail, szLog is Empty"));

        Log.Asset(Log.Instance.CallbackDict != null,
            "Log.WriteTraceLog Fail, you should init first");

        string szData = szLog;

        if (Log.Instance.CallbackDict.Keys.Contains(level))
            Log.Instance.CallbackDict[level](szData);
    }

    static public void Info(string szLog)
    {
        Log.WriteTraceLog(Log.TraceLogLevel.Info, szLog);
    }

    static public void Error(string szLog)
    {
        Log.WriteTraceLog(Log.TraceLogLevel.Error, szLog);
    }

    static public void Warning(string szLog)
    {
        Log.WriteTraceLog(Log.TraceLogLevel.Warning, szLog);
    }

    static public void DebugFormat (string szFormatLog, params object[] lstParams)
    {
#if __DEBUG_
#if UNITY_EDITOR
        Log.WriteTraceLog(TraceLogLevel.Debug, string.Format(szFormatLog, lstParams)); 
#endif
#endif
    }
}
