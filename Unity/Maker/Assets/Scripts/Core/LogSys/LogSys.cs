using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;
using System.Net;
using System.Collections;
using System.Collections.Generic;

// 1. 定制 Log：设定对应 Log 级别的回调；输入的格式为 XML（所有的 < 会被自动替换为 C）
// 2. 管理 Log File：在设定的 Log Folder 生成一个 Log File；清理 Log Folder 的过期的 Log File
public class LogSys : SingletonModule<LogSys>
{
	private string m_szFolderPath = null;
	private string m_szNowLogFilePath = null;
	private StreamWriter m_LogFileStreamWriter = null;

    private StringBuilder _CacheAppendString = new StringBuilder();
	private int m_nMaxCacheLogCount = 5;
	private int m_nCurrentCacheLogCount = 0;

    public override void Init()
    {
#if !UNITY_EDITOR
        // Unity Log 不输出堆栈
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
#endif

        // Unity 的 Debug.Log 绑定给 Log
        Log.Instance.SetLevelCallback(Log.TraceLogLevel.Debug, Debug.Log);
        Log.Instance.SetLevelCallback(Log.TraceLogLevel.Info, Debug.Log);
        Log.Instance.SetLevelCallback(Log.TraceLogLevel.Warning, Debug.LogWarning);
        Log.Instance.SetLevelCallback(Log.TraceLogLevel.Error, Debug.LogError);

        // Unity 的 Log 输出直接输入到文件之中
        Application.logMessageReceived += writeLogToFile;
    }

    public override void Uninit()
    {
        Application.logMessageReceived -= writeLogToFile;

        Log.Instance.ClearLevelCallback(Log.TraceLogLevel.Debug);
        Log.Instance.ClearLevelCallback(Log.TraceLogLevel.Info);
        Log.Instance.ClearLevelCallback(Log.TraceLogLevel.Warning);
        Log.Instance.ClearLevelCallback(Log.TraceLogLevel.Error);

        m_szFolderPath = null;
        m_szNowLogFilePath = null;
        if (m_LogFileStreamWriter != null)
        {
            m_LogFileStreamWriter.Close();
            m_LogFileStreamWriter = null;
        }
    }

    public override void Update()
    {

    }

    public void SetFolder(string szFolderPath)
    {
        Log.Asset(szFolderPath != null && szFolderPath != string.Empty && szFolderPath != "",
            "LogSys.SetFolder Fail, FolderPath is Empty");
        Log.Asset(m_szFolderPath == null, "LogSys.SetFolder Fail, _FolderPath is already Set");

        if (!Directory.Exists(szFolderPath))
        {
            Directory.CreateDirectory(szFolderPath);
        }

        m_szFolderPath = szFolderPath;
    }

    public void ClearOldLogs(double persistDays)
    {
        Log.Asset(persistDays > 1, string.Format("LogSys ClearOldLogs Fail, persistDays should bigger than 1: {0}", persistDays));
        Log.Asset(m_szFolderPath != null, "LogSys ClearOldLogs Fail, FolderPath is empty you should SetFolderPath first");

        DateTime deleteDays = DateTime.Now.AddDays(-persistDays); //在此日期的log都被删除
        string[] logFiles = Directory.GetFiles(m_szFolderPath, "*.txt");
        foreach (string file in logFiles)
        {
            if (file != m_szFolderPath)
            {
                if (File.GetCreationTime(file) < deleteDays)
                {
                    File.Delete(file);
                }
            }
        }
    }

    public void SetLogFile(string szLogFileName)
    {
        Log.Asset(szLogFileName != null && szLogFileName != string.Empty && szLogFileName != "",
            "LogSys SetLogFile Fail, LogFilePath is Empty");
        Log.Asset(m_szFolderPath != null && m_szFolderPath != string.Empty,
            "LogSys SetLogFile Fail, you should Set Folder First");

        // 先移除上一个
        if (m_LogFileStreamWriter != null)
        {
            try
            {
                m_LogFileStreamWriter.Close();
            }
            catch (System.Exception e)
            {
                Log.WriteTraceLog(Log.TraceLogLevel.Error,
                    string.Format("LogSys SetLogFile Fail, Close File Get Exception {0}", e.ToString()));
                return;
            }
            m_szNowLogFilePath = string.Empty;
        }

        m_szNowLogFilePath = string.Format("{0}/{1}", m_szFolderPath, szLogFileName);
        try
        {
            m_LogFileStreamWriter = new StreamWriter(m_szNowLogFilePath, true, Encoding.UTF8);
        }
        catch (System.Exception e)
        {
            Log.WriteTraceLog(Log.TraceLogLevel.Error,
                string.Format("LogSys SetLogFile Fail, New StreamWriter Get Exception {0}", e.ToString()));
        }
    }

    private static string currentTimeStr
    {
        get
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff   ");
        }
    }

    private void writeLogToFile(string logString, string stackTrace, LogType type)
    {
        if (m_LogFileStreamWriter != null)
        {
            bool bFlushAllCache = false;
            StringBuilder sb = _CacheAppendString;
            string szTypeName = type.ToString().ElementAt(0).ToString();
            sb.Append("\n<").Append(szTypeName).Append(">");
            sb.Append(LogSys.currentTimeStr + logString.Replace('<', 'C'));
            if (type == LogType.Error || type == LogType.Exception)
            {
                sb.Append("\n<S><![CDATA[\n").Append(stackTrace).Append("]]></S>\n");
                // 直接输出
                bFlushAllCache = true;
            }
            sb.Append("</").Append(szTypeName).Append(">\n");
            m_nCurrentCacheLogCount++;

            bFlushAllCache = bFlushAllCache || m_nMaxCacheLogCount <= m_nCurrentCacheLogCount;
            if (bFlushAllCache) 
            {
                FlushToFile();
            }
        }
    }

    public void FlushToFile ()
    {
        try
        {
            m_LogFileStreamWriter.Write(_CacheAppendString.ToString());
            m_LogFileStreamWriter.Flush();
        }
        catch (System.Exception E)
        {
            Log.WriteTraceLog(Log.TraceLogLevel.Error,
                string.Format("LogSys writeLogToFile Fail, File Write Get Exception {0}", E.ToString()));
        }

        // 清空
        _CacheAppendString.Remove(0, _CacheAppendString.Length);
        m_nCurrentCacheLogCount = 0;
    }

    public string[] GetAllLogsFileName()
    {
        Log.Asset(m_szFolderPath != string.Empty && m_szFolderPath != null, "LogSys.GetAllLogs Fail, FolderPath is Empty");
        string[] logFiles = Directory.GetFiles(m_szFolderPath, "*.txt");
        return logFiles.Select(x => Path.GetFileName(x)).OrderByDescending(x => x).ToArray();
    }

    public string GetNowLogPath()
    {
        Log.Asset(m_szNowLogFilePath != null && m_szNowLogFilePath != string.Empty, "LogSys.GetNowLogPath Fail, _NowLogFilePath is Empty");

        return m_szNowLogFilePath;
    }

    public string GetNowFolderPath ()
    {
        Log.Asset(m_szFolderPath != null && m_szFolderPath != string.Empty, "LogSys.GetNowFolderPath Fail, _FolderPath is Empty");

        return m_szFolderPath;
    }
}

