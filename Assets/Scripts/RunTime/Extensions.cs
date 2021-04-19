using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;


public static class MyExtensions
{
    public static T GetOrCreate<T>(this GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t)
        {
            return t;
        }

        return go.AddComponent<T>();
    }

    public static GameObject GetParent(this GameObject go)
    {
        return go.transform.parent ? go.transform.parent.gameObject : null;
    }

    public static void AttachChild(this GameObject go, GameObject child)
    {
        child.transform.SetParent(go.transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;
    }

    public static void RemoveChild(this GameObject go, GameObject child)
    {
        if (child.GetParent() == go)
        {
            child.transform.parent = null;
        }
    }

    public static void RemoveAllChildren(this GameObject go, bool immediately = false)
    {
        List<Transform> lst = new List<Transform>();
        foreach (Transform child in go.transform)
        {
            lst.Add(child);
        }
        for (int i = 0; i < lst.Count; i++)
        {
            if(!immediately)
                GameObject.Destroy(lst[i].gameObject);
            else
                GameObject.DestroyImmediate(lst[i].gameObject);
        }
    }

    public static GameObject FindChild(this GameObject go, string name)
    {
        Transform t = GetChild(go, name);
        if (t == null)
        {
            //Debugger.LogError("Not Found on " + go.name +"/" + name);
            return null;
        }
        else
        {
            return t.gameObject;
        }
        //return t == null ? null : t.gameObject;
    }

    public static T FindChildComponent<T>(this GameObject go, string name) where T : Component
    {
        Transform t = GetChild(go, name);
        if (t == null)
        {
            //Debugger.LogError("Not Found on " + go.name +"/" + name);
            return null;
        }
        else
        {
            return t.GetComponent<T>();
        }
        //return t == null ? null : t.gameObject;
    }

    public static Transform GetChild(this GameObject go, string name)
    {
        Transform child = go.transform.Find(name);
        if (child != null)
        {
            return child;
        }

        return GetChildRecurse(go.transform, name);
    }

    public static Transform GetChild(this Component comp, string name)
    {
        Transform tr = comp.transform.Find(name);
        if (tr != null)
        {
            return tr;
        }

        foreach (Transform child in comp.transform)
        {
            Transform t = GetChildRecurse(child, name);
            if (t != null)
            {
                return t;
            }
        }

        return null;
    }

    public static Transform GetChildRecurse(Transform trans, string name)
    {
        if (trans.name == name)
        {
            return trans;
        }

        foreach (Transform child in trans.transform)
        {
            Transform t = GetChildRecurse(child, name);
            if (t != null)
            {
                return t;
            }
        }

        return null;
    }
    static public void SetLayer(this GameObject go, int layer)
    {
        SetLayer(go.transform, layer);
    }

    static public void SetLayer(this Transform t, int layer)
    {
        t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            child.gameObject.layer = layer;
            SetLayer(child, layer);
        }
    }

    static public void SetLayer(this Transform t, int layer, int exceptLayerMask)
    {
        int oLayerMask = 1 << t.gameObject.layer;
        if ((oLayerMask & exceptLayerMask) == 0)
            t.gameObject.layer = layer;
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child, layer, exceptLayerMask);
        }
    }

    static public void SetTag(this GameObject go, string tag)
    {
        SetTag(go.transform, tag);
    }

    static public void SetTag(this Transform t, string tag)
    {
        t.gameObject.tag = tag;
        for (int i = 0; i < t.childCount; ++i)
        {
            Transform child = t.GetChild(i);
            child.gameObject.tag = tag;
            SetTag(child, tag);
        }
    }

    public static Color WithA(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    public static Vector3 GetVector3(this Color color)
    {
        return new Vector3(color.r, color.g, color.b);
    }

    public class UserDataComponent : MonoBehaviour
    {
        public object data;
    }

    public static object GetUserData(this GameObject go)
    {
        UserDataComponent c = go.GetComponent<UserDataComponent>();
        if (c != null)
        {
            return c.data;
        }
        return null;
    }

    public static object GetUserData(this Component c)
    {
        return c.gameObject.GetUserData();
    }

    public static void SetUserData(this GameObject go, object data)
    {
        UserDataComponent c = go.GetComponent<UserDataComponent>();
        if (c == null)
        {
            c = go.AddComponent<UserDataComponent>();
        }

        c.data = data;
    }

    public static void SetUserData(this Component c, object data)
    {
        c.gameObject.SetUserData(data);
    }

    public static void SetParent(this GameObject _go, Transform _par)
    {
        if (_go != null)
        {
            _go.transform.SetParent(_par);
            _go.transform.localPosition = Vector3.zero;
            _go.transform.localRotation = Quaternion.identity;
            _go.transform.localScale = Vector3.one;
        }
    }

    public static void BubbleSort<T>(this List<T> list, Func<T, T, int> comp)
    {
        for (int i = list.Count; i > 0; i--)
        {
            for (int j = 0; j < i - 1; j++)
            {
                int ret = comp(list[j], list[j + 1]);
                if (ret > 0)
                {
                    var temp = list[j];
                    list[j] = list[j + 1];
                    list[j + 1] = temp;
                }
            }
        }
    }

    public static void ShowObject(this GameObject go, bool show)
    {
        if (null == go) return;
        Renderer selfRenderer = go.GetComponent<Renderer>();
        if (null != selfRenderer)
            selfRenderer.enabled = show;

        Renderer[] childrenRenderers = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < childrenRenderers.Length; ++i)
            childrenRenderers[i].enabled = show;

        
    }

    public static void Clear<T>(this Action<T> action)
    {
        System.Delegate[] dels = action.GetInvocationList();
        for (int i = 0; i < dels.Length; i++)
        {
            action -= dels[i] as Action<T>;
        }
    }

    public static void SetActive(this Transform trans, bool active)
    {
        trans.gameObject.SetActive(active);
    }

}
