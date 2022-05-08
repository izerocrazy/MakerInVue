using System.IO;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

class PerformBuild
{
	static string[] GetBuildScenes()
	{
		List<string> names = new List<string>();

		foreach(EditorBuildSettingsScene e in EditorBuildSettings.scenes)
		{
			if(e==null)
				continue;

			if(e.enabled)
				names.Add(e.path);
		}
		return names.ToArray();
	}

	static string GetBuildPath()
	{
		return "build/iPhone" + System.DateTime.Now.ToString();
	}

	// [UnityEditor.MenuItem("Project Cos/Resources/Command Line Build Step")]
	static void CommandLineBuild ()
	{

		Debug.Log("Command line build\n------------------\n------------------");

		string[] scenes = GetBuildScenes();
		string path = GetBuildPath();
		if(scenes == null || scenes.Length==0 || path == null)
			//return;
			Debug.Log(string.Format("Path: \"{0}\"", path));
		for(int i=0; i<scenes.Length; ++i)
		{
			Debug.Log(string.Format("Scene[{0}]: \"{1}\"", i, scenes[i]));
		}

		Debug.Log("Starting Build!");
		BuildPipeline.BuildPlayer(scenes, path, BuildTarget.iOS, BuildOptions.None);
		Debug.Log ("Build Finish");
	}

	static string GetBuildPathAndroid()
	{
		return "build/android";
	}
}
