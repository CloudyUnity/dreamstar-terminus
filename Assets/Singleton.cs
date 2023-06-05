using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0162 // Unreachable code detected
public class Singleton : MonoBehaviour
{
    public static Dictionary<System.Type, Singleton> Instances = new Dictionary<System.Type, Singleton>();

    const bool DEBUG_MODE = false;

    protected virtual void Awake()
    {
        if (Instances.ContainsValue(null))
            Instances.Clear();

        if (DEBUG_MODE)
            Debug.Log("Checking type: " + GetType());

        if (!Instances.ContainsKey(GetType()))
        {
            if (DEBUG_MODE)
                Debug.Log("Instancing: " + GetType());

            Instances.Add(GetType(), this);
        }
        else
        {
            if (DEBUG_MODE)
                Debug.Log("Already instanced as: " + Instances[GetType()]);

            Destroy(gameObject);
        }
    }

    public static void ShowInside()
    {
        Debug.Log("Showing Inside:");
        foreach (var key in Instances.Keys)
        {
            Debug.Log(key + " - " + Instances[key]);
        }
    }

    public static T Get<T>()
        where T : Singleton
    {
        if (DEBUG_MODE)
            Debug.Log("Trying to get: " + typeof(T).Name);

        if (!Instances.ContainsKey(typeof(T)))
            return null;

        if (DEBUG_MODE)
            Debug.Log("Value: " + Instances[typeof(T)]);

        if (Instances[typeof(T)] is T instance)
            return instance;

        throw new System.Exception("Instances contains mismatched key-value pair: " + typeof(T).Name + " " + Instances[typeof(T)].name);
    }
}
#pragma warning restore CS0162 // Unreachable code detected
