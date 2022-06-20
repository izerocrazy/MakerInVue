using System;
using UnityEngine.UI;

public class GameScene: BaseScene
{
    public GameScene()
    {
    }

	public override void OnMainRunning()
	{
		Log.Info("GameScene OnMainRunning");

		LuaMachine m = Main.Instance.GetModule("LuaMachine") as LuaMachine;

		m.DoFile("LuaFile/Game/main");
	}

	// Update is called once per frame
	void Update()
	{
		base.Update();
	}
}
