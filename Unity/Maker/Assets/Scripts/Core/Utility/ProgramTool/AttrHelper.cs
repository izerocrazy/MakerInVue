using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

public class AttributeHelper
{
    public static Attr GetCustomAttribute<Master, Attr>()
        where Attr : Attribute
    {
        System.Reflection.MemberInfo info = typeof(Master);
        Attr att = (Attr)Attribute.GetCustomAttribute(info, typeof(Attr));
        return att;
    }

    public static Attr GetCustomAttribute<Attr>(Type type)
        where Attr : Attribute
    {
        System.Reflection.MemberInfo info = type;
        Attr att = (Attr)Attribute.GetCustomAttribute(info, typeof(Attr));
        return att;
    }

    public static Attr[] GetCustomAttributes<Master, Attr>()
        where Attr : Attribute
    {
        System.Reflection.MemberInfo info = typeof(Master);
        Attr[] att = (Attr[])Attribute.GetCustomAttributes(info, typeof(Attr));
        return att;
    }

    public static Attr[] GetCustomAttributes<Attr>(Type type)
        where Attr : Attribute
    {
        System.Reflection.MemberInfo info = type;
        Attr[] att = (Attr[])Attribute.GetCustomAttributes(info, typeof(Attr));
        return att;
    }

    public static void OnFieldsCallWithAttributes<Attr>(object owner, Action<FieldInfo> action)
        where Attr : Attribute
    {
        Log.Asset(owner != null,
            "AttributeHelper OnFieldsCallWithAttributes Fail, owner is Empty");
        Type type = owner.GetType();
        var fields = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        Log.Asset(fields.Length > 0,
            "AttributeHelper:OnFieldsCallWithAttributes Fail, the fields is empty");

        // 遍历
        foreach (var field in fields)
        {
            Attr creator = (Attr)Attribute.GetCustomAttribute(field, typeof(Attr));
            if (creator != null)
            {
                action(field);
            }
        }
    }

    public static void CallMethodsWithAttributes<Attr>(object owner, object[] parameters)
        where Attr : Attribute
    {
        Log.Asset(owner != null,
            "AttributeHelper OnMethodsCallWithAttributes Fail, owner is Empty");
        Type type = owner.GetType();
        var methods = type.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        Log.Asset(methods.Length > 0,
            "AttributeHelper:OnMethodsCallWithAttributes Fail, the methods is Empty");

        foreach (var method in methods)
        {
            Attr creator = (Attr)Attribute.GetCustomAttribute(method, typeof(Attr));
            if (creator != null)
            {
                method.Invoke(owner, parameters);
            }
        }
    }
}