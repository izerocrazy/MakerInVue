using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using XLua;

[CSharpCallLua]
public delegate void LuaCallByGoDelegate(LuaComponent go);

[CSharpCallLua]
public delegate void LuaCallWithParamObj(params object[] param);

/// 
/// Lua组件 - 它调用的Lua脚本可以实现类似MonoBehaviour派生类的功能
/// 
[LuaCallCSharp]
public class LuaComponent : MonoBehaviour
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
    LuaFunction LuaOnDestroyCall;
    GameObject mGo;
    bool IsRun = false;

    public static LuaTable GetLuaComponent(GameObject go)
    {
        LuaComponent luaComp = go.GetComponent<LuaComponent>();
        if (luaComp == null)
            return null;
        return luaComp.LuaModule;
    }

    public static LuaTable GetLuaComponent(GameObject go, string luaFile)
    {
        LuaComponent[] comps = go.GetComponents<LuaComponent>();
        if (comps == null || comps.Length == 0)
            return null;

        foreach(var comp in comps)
        {
            if (comp.LuaFilePath == luaFile)
                return comp.LuaModule;
        }

        return null;
    }

    public static LuaTable GetOrAddLuaComponent(GameObject go, string childPath, string luaFile)
    {
        return GetOrAddLuaComponent(go.transform, childPath, luaFile);
    }

    public static LuaTable GetOrAddLuaComponent(Transform trans, string childPath, string luaFile)
    {
        var child = trans.Find(childPath);
        return GetOrAddLuaComponent(child, luaFile);
    }

    public static LuaTable GetOrAddLuaComponent(Transform trans, string luaFile)
    {
        return GetOrAddLuaComponent(trans.gameObject, luaFile);
    }

    public static LuaTable GetOrAddLuaComponent(GameObject go, string luaFile)
    {
        var result = GetLuaComponent(go, luaFile);
        if (result != null)
            return result;

        return AddLuaComponent(go, luaFile);
    }

    public static LuaTable AddLuaComponent(Transform tran, string childPath, string luaFile)
    {
        //Debug.Log("FindChild:"+tran.name+"=>"+childPath);
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
        LuaComponent luaComp = go.AddComponent<LuaComponent>();
        luaComp.LuaFilePath = luaFile;
        luaComp.RunLuaFile(luaFile);
        return luaComp.LuaModule;
    }

    public static LuaTable HasLuaComponent(GameObject go, string luaFile)
    {
        LuaComponent[] luaComps = go.GetComponents<LuaComponent>();
        if (luaComps.Length > 0)
        {
            for (int index = 0; index < luaComps.Length; index++)
            {
                LuaComponent comp = luaComps[index];
                if (comp.LuaFilePath == luaFile)
                {
                    return comp.LuaModule;
                }
            }
        }
        return null;
    }

    public static void RemoveLuaComponent(GameObject go)
    {
        LuaComponent[] luaComps = go.GetComponents<LuaComponent>();
        if (luaComps.Length > 0)
        {
            for (int luaCompIndex = luaComps.Length - 1; luaCompIndex >= 0; luaCompIndex--)
            {
                Destroy(luaComps[luaCompIndex]);
            }
        }
    } 

    protected void RunLuaFile(string luaFile)
    {
        if (string.IsNullOrEmpty(luaFile)||IsRun)
            return;
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
            this.LuaModule.Get("OnDestroy", out LuaOnDestroyCall);
            this.LuaModule.Get("OnEnable", out LuaOnEnable);

            if (LuaAwakeCall != null)//放到此处为顾及通过AddLuaComponent添加时调用
            {
                LuaAwakeCall.Call(this);
                //Debug.Log("回调Awake:"+gameObject.name);
            }
        }
        else
        {
            Debug.LogError("Lua脚本没有返回Table对象：" + LuaFilePath);
        }
    }

    public void RemoveLuaInfo()
    {
        LuaOnDestroyCall = null;
        this.LuaModule = null;
    }

    //void Awake()
    //{
    //    RunLuaFile(LuaFilePath);
    //}

    void OnEnable()
    {
        if (LuaOnEnable != null)
            LuaOnEnable.Call(this);
    }
    
    void Start()
    {
        if (LuaStartCall != null)
            LuaStartCall.Call(this);
        
    }

    //void Update()//为了优化性能，该组件不使用Update方法，如果使用该方法，请使用LuaComponentWithUpdate 组件
    //{
    //    if (LuaUpdateCall != null)
    //        LuaUpdateCall.Call();
    //}

    Animation anim;
    void OnDestroy()
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
        //if (LuaOnDestroyCall != null) LuaOnDestroyCall.Dispose();
        LuaAwakeCall = null;
        LuaOnEnable = null;
        LuaStartCall = null;
        LuaOnDestroyCall = null;
        ClearClick();
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
            luafunc = null;
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
        foreach (var de in mButtons)
        {
            if (de.Value != null)
            {
                //de.Value.Dispose();
            }
        }
        mButtons.Clear();
    }

    
}

