using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SingletonObject <T> : ISingletonObject 
    where T : ISingletonObject, new()
{
    private static T _instance;

    public static T Instance
    {
        // 创建一个 T 实例
        get
        {
            if (_instance == null &&
                // 不包含 AutoNew 标签的，或者 AutoNew 为 true 的会自动新建 
                (AttributeHelper.GetCustomAttribute<AutoNewAttribute>(typeof(T)) == null ||
                (AttributeHelper.GetCustomAttribute<AutoNewAttribute>(typeof(T)) != null &&
                AttributeHelper.GetCustomAttribute<AutoNewAttribute>(typeof(T)).AutoNew))) 
            {
                CreateInstance();
            }

            return _instance;
        }
    }

    public static void CreateInstance ()
    {
        if (_instance != null)
        {
            throw new Exception("SingletonObject CreateInstance Fail, already Have Instance");
        }

        _instance = new T();
        _instance.Init();
    }

    public virtual void Init ()
    {
        throw new Exception("Should Override Init");
    }

    public virtual void Uninit ()
    {
        throw new Exception("Should Override Uninit");
    }
}
