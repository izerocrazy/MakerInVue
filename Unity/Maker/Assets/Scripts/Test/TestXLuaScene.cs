using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestXLuaScene : BaseScene {

	// Use this for initialization
	void Start () {
		
	}

	public override void OnMainRunning ()
	{
		Log.Info ("TestXLuaScene OnMainRunning");

		Text text = Utility.Find ("Canvas.Text").GetComponent("Text") as Text;
		Log.Asset (text != null);
		text.text = "Hello World For C#";

		LuaMachine m = Main.Instance.GetModule ("LuaMachine") as LuaMachine;

		// Test DoString (C# Call Lua Function)
		m.DoString ("print('Hello World')");

		// Test DoFile and Test Lua Call C#
		m.DoFile ("LuaFile/Test/TestXLua.lua");

		// Test Lua Call C#
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
	}

	public void TestLuaCall ()
	{
		Log.Info ("TestLuaCall");
	}
}
