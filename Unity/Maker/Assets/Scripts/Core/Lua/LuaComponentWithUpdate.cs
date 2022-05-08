using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using XLua;

[LuaCallCSharp]
public class LuaComponentWithUpdate : MonoBehaviour
{
    public string LuaFilePath;

    private Dictionary<GameObject, LuaFunction> mButtons;

    public LuaTable LuaModule
    {
        get;
        private set;
    }

    LuaFunction LuaAwakeCall;
    LuaFunction LuaOnEnable;
    LuaFunction LuaStartCall;
    LuaFunction LuaUpdateCall;
    LuaFunction LuaOnDestroyCall;
    GameObject mGo;
    bool IsRun = false;

    public static LuaTable GetLuaComponent(GameObject go)
    {
        LuaComponentWithUpdate luaComp = go.GetComponent<LuaComponentWithUpdate>();
        if (luaComp == null)
            return null;
        return luaComp.LuaModule;
    }

    public static LuaTable AddLuaComponent(Transform tran,string childPath, string luaFile)
    {
#if UNITY_5_6
        return AddLuaComponent(tran.Find(childPath).gameObject, luaFile);
#else
         return AddLuaComponent(tran.Find(childPath).gameObject, luaFile);
#endif

    }

    public static LuaTable AddLuaComponent(Transform tran, string luaFile)
    {
        return AddLuaComponent(tran.gameObject, luaFile);
    }

    public static LuaTable AddLuaComponent(GameObject go, string luaFile)
    {
        LuaComponentWithUpdate luaComp = go.AddComponent<LuaComponentWithUpdate>();
        luaComp.LuaFilePath = luaFile;
        luaComp.RunLuaFile(luaFile);
        return luaComp.LuaModule;
    }
    public static void RemoveLuaComponent(GameObject go)
    {
        LuaComponentWithUpdate[] luaComps = go.GetComponents<LuaComponentWithUpdate>();
        if (luaComps.Length > 0)
        {
            for (int luaCompIndex = luaComps.Length - 1; luaCompIndex >= 0; luaCompIndex--)
            {
                Destroy(luaComps[luaCompIndex]);
            }
        }
    }
    void RunLuaFile(string luaFile)
    {
        if (string.IsNullOrEmpty(luaFile)||IsRun)
            return;
        //Log.Info ("RunLunFile ----->>luaFile =" + luaFile+ "IsRun  "+ IsRun);
        IsRun = true;
        LuaMachine machine = Main.Instance.GetSingletonModule<LuaMachine>();
        Log.Asset(machine != null && machine.Env != null, "LuaComponent RunLuaFile Fail, LuaMachine is Empty");
        object[] luaRet = machine.DoFile(luaFile);// s_luaState.DoString(luaText, luaFile);
        if (luaRet != null && luaRet.Length >= 1)
        {
            // 约定：第一个返回的Table对象作为Lua模块
            this.LuaModule = luaRet[0] as LuaTable;
            //-- 取得常用的函数回调
            mGo = this.gameObject;
            this.LuaModule.Get("Awake",out LuaAwakeCall);
            this.LuaModule.Get("Start",out LuaStartCall);
            this.LuaModule.Get("Update",out LuaUpdateCall);
            this.LuaModule.Get("OnDestroy", out LuaOnDestroyCall);
            this.LuaModule.Get("OnEnable", out LuaOnEnable);

            TrySetModule();

            if (LuaAwakeCall != null)//放到此处为顾及通过AddLuaComponent添加时调用
            {
                LuaAwakeCall.Call(this);
                //Debug.Log("回调Awake:"+gameObject.name);
            }

            if (LuaOnEnable != null)
            {
                LuaOnEnable.Call(this);
            }
        }
        else
        {
            Debug.LogError("Lua脚本没有返回Table对象：" + LuaFilePath);
        }
    }

    const string moduleSpecialStr = "Modules/Mod_";
    const string GetModuleScript = 
        @"
            return UIManager:GetModule('{0}')
        ";
    //如果它是放在一个module中的，尝试设置它的module属性
    void TrySetModule()
    {
        if (LuaModule == null)
            return;
        var path = LuaFilePath.Replace('\\', '/');
        if (LuaFilePath.StartsWith(moduleSpecialStr))
        {
            int start = moduleSpecialStr.Length;
            int end = LuaFilePath.IndexOf("/", start);
            string moduleName = LuaFilePath.Substring(moduleSpecialStr.Length, end - start);
            LuaEnv env = LuaMachine.Instance.Env;
            if (env != null)
            {
                string s = string.Format(GetModuleScript, moduleName);
                LuaTable m = env.DoString(s, "LuaComponentWithUpdate:GetModuleScript")[0] as LuaTable;
                if (m != null)
                    LuaModule.Set("module", m);
            }
        }
    }

    //void Awake()
    //{
    //    RunLuaFile(LuaFilePath);
    //}

    protected void OnEnable()
    {
        if (LuaOnEnable != null)
            LuaOnEnable.Call(this);
    }
    
    void Start()
    {
        if (LuaStartCall != null)
            LuaStartCall.Call(this);

        //LuaManager.Instance.DoString("require'LuaFile/Test'");
    }

    void Update()
    {
        if (LuaUpdateCall != null)
            LuaUpdateCall.Call();
    }
    Animation anim;
    void OnDestroy()
    {
        OnClear();
    }

    public void OnClear()
    {
        if (Main.Instance.IsHaveSingletonModule<LuaMachine>())
        {
            if (LuaOnDestroyCall != null)
                LuaOnDestroyCall.Call(this);
            if (this.LuaModule != null)
                this.LuaModule.Dispose();
        }
        //if (LuaAwakeCall != null) LuaAwakeCall.Dispose();
        //if (LuaOnEnable != null) LuaOnEnable.Dispose();
        //if (LuaStartCall != null) LuaStartCall.Dispose();
        //if (LuaUpdateCall != null) LuaUpdateCall.Dispose();
        //if (LuaOnDestroyCall != null) LuaOnDestroyCall.Dispose();
        LuaAwakeCall = null;
        LuaOnEnable = null;
        LuaStartCall = null;
        LuaUpdateCall = null;
        LuaOnDestroyCall = null;
        ClearClick();
        IsRun = false;
    }

    public void RunLuaFile()
    {
        RunLuaFile(LuaFilePath);
    }

    public void RemoveLuaInfo ()
    {
        LuaOnDestroyCall = null;
        this.LuaModule = null;
    }

    /// <summary>
    /// 添加单击事件
    /// </summary>
    public void AddClick(GameObject go, LuaFunction luafunc)
    {
        if (mButtons == null)
            mButtons = new Dictionary<GameObject, LuaFunction>();

        if (go == null || luafunc == null) return;
        //Debug.Log("AddClickCallBack:" + go.name);
        if (mButtons.ContainsKey(go))
            return;
        mButtons.Add(go, luafunc);
        go.GetComponent<Button>().onClick.AddListener(
            delegate ()
            {
                luafunc.Call(go);
            }
        );
    }
    
    /// <summary>
    /// 添加单击事件
    /// </summary>
    public void AddClick(string btnName, LuaFunction luafunc)
    {
#if UNITY_5_6
        this.AddClick(transform.Find(btnName).gameObject, luafunc);
#else
        this.AddClick(transform.Find(btnName).gameObject, luafunc);
#endif

    }

    //    /// <summary>
    //    /// 添加单击事件
    //    /// </summary>
    //    public void AddClick(string btnName, LuaFunction luafunc)
    //    {
    //        this.transform.FindChild()
    //        if (mButtons == null)
    //            mButtons = new Dictionary<GameObject, LuaFunction>();
    //
    //        if (go == null || luafunc == null) return;
    //        //Debug.Log("AddClickCallBack:" + go.name);
    //        if (mButtons.ContainsKey(go))
    //            return;
    //        mButtons.Add(go, luafunc);
    //        go.GetComponent<Button>().onClick.AddListener(
    //            delegate ()
    //            {
    //                luafunc.Call(go);
    //            }
    //        );
    //    }

    /// <summary>
    /// 删除单击事件
    /// </summary>
    /// <param name="go"></param>
    public void RemoveClick(GameObject go)
    {
        if (go == null || mButtons == null) return;
        LuaFunction luafunc = null;
        if (mButtons.TryGetValue(go, out luafunc))
        {
            //luafunc.Dispose();
            //luafunc = null;
            mButtons.Remove(go);
        }
    }

    /// <summary>
    /// 清除单击事件
    /// </summary>
    public void ClearClick()
    {
        if (mButtons == null)
            return;
        //foreach (var de in mButtons)
        //{
        //    if (de.Value != null)
        //    {
        //        de.Value.Dispose();
        //    }
        //}
        mButtons.Clear();
    }


    //protected void CallLuaFunction(string funcName, params object[] args)
    //{
    //    if (this.LuaModule == null)
    //        return;
    //    LuaFunction func = this.LuaModule[funcName] as LuaFunction;
    //    if (func != null)
    //        func.Call(args);
    //}
}

