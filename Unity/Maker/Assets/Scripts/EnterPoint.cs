using UnityEngine;

public class EnterPoint : MonoBehaviour
{
	public GameObject SceneObj = null;

    public void Start()
    {
        DoNormalStart();
    }

    private void DoNormalStart()
    {
        // 如果还没有启动 GameMain，那么就需要启动 GameMain
        GameObject GameMainGO = GameObject.Find("BaseScene(Clone)");
        if (GameMainGO == null)
        {
            // 通过 SceneObj 启动 GameMain
			if (SceneObj == null) {
				GameObject newPanelObj = GameObject.Instantiate (Resources.Load ("Prefabs/BaseScene")) as GameObject;
				Log.Asset (newPanelObj != null);
			} else {
				GameObject newPanelObj = GameObject.Instantiate (SceneObj) as GameObject;
				Log.Asset (newPanelObj != null);
			}
        }
    }

    public void OnDestroy ()
    {
        // 阻止 BaseScene 之中的 OnDestroy() 的关闭 GameMain 逻辑
    }

    [ContextMenu("ShowUserPath")]
    void ShowUserPath()
    {
        Application.OpenURL(Application.persistentDataPath);
    }
}

