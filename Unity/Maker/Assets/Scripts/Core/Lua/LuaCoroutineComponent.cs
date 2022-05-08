using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System;

[LuaCallCSharp]
public class LuaCoroutineComponent : MonoBehaviour
{
    static LuaCoroutineComponent coroutineObj;
    static LuaCoroutineComponent CoroutineObj { get
        { if (coroutineObj == null) {
                coroutineObj = new GameObject("coroutineObj").AddComponent<LuaCoroutineComponent>();
                DontDestroyOnLoad(coroutineObj.gameObject);
            }
            return coroutineObj;
        }
        
    }

    public static void YieldAndCallback(object to_yield, LuaFunction callback,GameObject go)
    {
        if (go != null)
        {
            LuaCoroutineComponent com = go.GetComponent<LuaCoroutineComponent>();
            if (com == null)
                com = go.AddComponent<LuaCoroutineComponent>();
            com.StartCoroutine(com.CoBody(to_yield, callback));
        }
        else
        {
            CoroutineObj.StartCoroutine(CoroutineObj.CoBody(to_yield, callback));
        }

        
    }

    public IEnumerator CoBody(object to_yield, LuaFunction callback)
    {
        if (to_yield is IEnumerator)
            yield return StartCoroutine((IEnumerator)to_yield);
        else
            yield return to_yield;
        if (callback is LuaFunction)
        {
            callback.Call();
        }
        else
        {
            if (callback != null)
            {
                Log.Error("CoBody error :" + callback.GetType().ToString());
            }
        }
    }
}

