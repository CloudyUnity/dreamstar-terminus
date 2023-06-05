using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Dictionary<System.Type, Singleton> Instances = new Dictionary<System.Type, Singleton>();

    private void Awake()
    {
        // GIT TEST 1

        if (Instances.ContainsValue(null))
            Instances.Clear();

        //Debug.Log("Checking type: " + GetType());

        if (!Instances.ContainsKey(GetType()))
        {
            Debug.Log("Instancing: " + GetType());
            Instances.Add(GetType(), this);
        }
        else
        {
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
        //Debug.Log("A: " + typeof(T).Name);

        if (!Instances.ContainsKey(typeof(T)))
            return null;

        //Debug.Log("B: " + Instances[typeof(T)]);

        if (Instances[typeof(T)] is T instance)
            return instance;

        //Debug.Log("C: null");

        return null;
    }
}
