using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

public class BuildSystem
{
    public static string LuaFolderPath = Path.GetFullPath(Path.Combine(Application.dataPath, "./Resources/Lua/"));

    // [MenuItem("Tools/打包 Lua Txt")]
    public static void BuildLuaTextFile ()
    {
        // 先准备所有 Lua File 为 TxtFile
        // AllLuaFileToTxt();
    } 

	/*
    private static string[] AllLuaFileToTxt ()
    {
        // 获取文件夹下 lua 文件
        string [] lstFile = FileHelper.GetFileNames(LuaFolderPath, "*.lua", true);
        List<string> lstRetFile = new List<string>();

        foreach (string path in lstFile)
        {
            string szTxtFilePath = path + ".txt";
            // 复制为 .txt
            if (FileHelper.IsExistFile (path))
            {
                FileHelper.Copy(path, szTxtFilePath);
                lstRetFile.Add(szTxtFilePath);
                Debug.Log("Copy " + path + " >> " + szTxtFilePath);
            }
        }

        AssetDatabase.Refresh();

        return lstRetFile.ToArray();
    }

    private static void RemoveAllLuaTxtFile ()
    {
        // 获取文件夹下 lua 文件
        string [] lstFile = FileHelper.GetFileNames(LuaFolderPath, "*.lua.txt", true);

        foreach (string path in lstFile)
        {
            Debug.Log("Delete: " + path);
            FileHelper.DeleteFile(path);
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/打包 Lua AssetBundle")]
    public static void BuildLuaAssetBundle ()
    {
        string szAssetsBundlePath = Application.streamingAssetsPath + "/AssetsBundle.ab";
        // 清理掉上一次的内容
        ClearAllAssetBundleName();
        FileHelper.DeleteDirectory(szAssetsBundlePath);
        FileHelper.DeleteDirectory(Application.streamingAssetsPath);
        FileHelper.CreateDirectory(Application.streamingAssetsPath);
        FileHelper.CreateDirectory(szAssetsBundlePath);
        AssetDatabase.Refresh();

        // 先准备所有 Lua File 为 TxtFile
        string[] szFilePathArray = AllLuaFileToTxt();

        AssetDatabase.Refresh();
        // 将 TxtFile 加上 AssetBundle 标记
        foreach (string szFilePath in szFilePathArray)
        {
            string szTemp = szFilePath.Remove(0, Application.dataPath.Length - "Assets".Length);
            AssetImporter importer = AssetImporter.GetAtPath(szTemp);
            Log.Asset(importer != null, "Get AssetImporter Fail, the File Path is :"+ szTemp);
            importer.assetBundleName = szTemp.Remove(0, "Assets/resources/".Length).ToLower();
        }
        AssetDatabase.Refresh();

        // 输出 AssetBundle（如果是 prefab 这样的资源，还需要处理下引用的关系）
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(szAssetsBundlePath, BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);
        foreach (string szPath in manifest.GetAllAssetBundles())
        {
            Debug.Log(string.Format("{0}", szPath));
        }
        Debug.Log("Finish Build Asset Bundles");

        // 移除所有 TxtFile
        RemoveAllLuaTxtFile();

        ClearAllAssetBundleName();
        AssetDatabase.Refresh();

        AssetBundle ab = AssetBundle.LoadFromFile(szAssetsBundlePath + "/lua/gamemain.lua.txt");
        Debug.Log(szAssetsBundlePath + "/lua/gamemain.lua.txt");
        foreach (string szPath in ab.GetAllAssetNames())
        {
            Debug.Log(szPath);
        }
        TextAsset file = (TextAsset)ab.LoadAsset("assets/resources/" + "lua/gamemain.lua.txt".ToLower(), typeof(TextAsset));
        if (file == null)
        {
            Debug.Log("Load File Fail");
        }
    }

    //清掉所有的assetbundle标记
    static void ClearAllAssetBundleName()
    {
        string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        string assetBundleNamesContent = string.Join("\n", assetBundleNames);
        Debug.Log("ClearAllAssetBundleName count : " + assetBundleNames.Length + " Content : \n" + assetBundleNamesContent);
        foreach (string assetBundleName in assetBundleNames)
        {
            AssetDatabase.RemoveAssetBundleName(assetBundleName, true);
        }
    }
    */
}
