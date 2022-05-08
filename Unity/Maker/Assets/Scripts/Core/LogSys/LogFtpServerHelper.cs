using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;
using System.Net;
using System.Collections;
using System.Collections.Generic;

public static class LogFTPServerHelper
{
    public static string FtpServerIP = "FtpServerIP";
    public static string FtpUserID = "FtpUserID";
    public static string FtpPassword = "FtpPassword";
    //public static string FtpServerIP = "rizhi.887gz.com";//"rizhi.vipbelle.com";
    //public static string FtpUserID = "rizhi";//"oss";
    //public static string FtpPassword = "Pinpin@168";//"Belle@168";

    public static int UploadCurrentLog(string uploadFileHeadName)
    {
        Log.Asset(uploadFileHeadName != null, "LogFTPServerHelper.UploadCurrentLog Fail, Head Name is Empty");

        string szFilePath = Main.Instance.GetSingletonModule<LogSys>().GetNowLogPath();
        Log.Asset(szFilePath != null, "LogFTPServerHelper.UploadCurrentLog Fail, you should set log file first");

        // 把缓存都写入文件
        Main.Instance.GetSingletonModule<LogSys>().FlushToFile();

        string versionCode = Resources.Load<TextAsset>("VersionCode").text;
        string targetFileName = string.Format("{0}-{1}-{2}", versionCode, uploadFileHeadName, Path.GetFileName(szFilePath));
        return UploadFtp(szFilePath, targetFileName);
    }

    public static int UploadLog(string uploadFileHeadName, string logFileName)
    {
        Log.Asset(logFileName != null, "LogFTPServerHelper.UploadLog Fail, logFileName is Empty");
        Log.Asset(uploadFileHeadName != null, "LogFTPServerHelper.UploadLog Fail, Head Name is Empty");

        string appID = Resources.Load<TextAsset>("AppId").text;
        string versionCode = Resources.Load<TextAsset>("VersionCode").text;
        string targetFileName = string.Format("{0}-{1}-{2}-{3}", appID, versionCode, uploadFileHeadName, Path.GetFileName(logFileName));

        string szFolderPath = Main.Instance.GetSingletonModule<LogSys>().GetNowFolderPath();
        Log.Asset(szFolderPath != null, "LogFTPServerHelper.UploadLog Fail, FolderPath is Empty");
        string srcFilePath = Path.Combine(szFolderPath, logFileName);

        // 这个文件有可能是今天的日志，需把缓存都写入文件
        Main.Instance.GetSingletonModule<LogSys>().FlushToFile();

        return UploadFtp(srcFilePath, targetFileName);
    }

    public static int UploadFtp(string srcFilePath, string ftpFileName)
    {
        string ftpServerIP = FtpServerIP;
        string ftpUserID = FtpUserID;
        string ftpPassword = FtpPassword;
        Log.Asset(ftpServerIP != null);
        Log.Asset(ftpUserID != null);
        Log.Asset(ftpPassword != null);

        FileInfo fileInf = new FileInfo(srcFilePath);
        Uri ftpUri = new Uri(string.Format("ftp://{0}/Log/16/{1}", ftpServerIP, ftpFileName));
        Debug.Log(string.Format("Ftp upload {0} to {1}", srcFilePath, ftpUri));

        FtpWebRequest reqFTP = (FtpWebRequest)WebRequest.Create(ftpUri);
        int result;
        try
        {
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.KeepAlive = false;
            reqFTP.Method = "STOR";
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            FileStream fs = fileInf.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Stream strm = reqFTP.GetRequestStream();
            for (int contentLen = fs.Read(buff, 0, buffLength); contentLen != 0; contentLen = fs.Read(buff, 0, buffLength))
            {
                strm.Write(buff, 0, contentLen);
            }
            strm.Close();
            fs.Close();
            result = 0;
        }
        catch (Exception ex)
        {
            reqFTP.Abort();
            Log.WriteTraceLog(Log.TraceLogLevel.Error, ex.Message + ex.StackTrace);
            result = -2;
        }
        return result;
    }
}
