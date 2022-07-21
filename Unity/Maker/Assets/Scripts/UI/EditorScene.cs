using System;
using UnityEngine.UI;

public class EditorScene: BaseScene
{
    public EditorScene()
    {
    }

	public override void OnMainRunning()
	{
		Log.Info("GameScene OnMainRunning");

		LuaMachine m = Main.Instance.GetModule("LuaMachine") as LuaMachine;

		m.DoFile("LuaFile/Editor/main");
		m.DoString("MainInit()");
	}

	// Update is called once per frame
	void Update()
	{
		base.Update();
	}

    private void OnDestroy()
    {
		LuaMachine m = Main.Instance.GetModule("LuaMachine") as LuaMachine;
		m.DoString("MainUninit()");
    }
}
