using SD.Tools.Algorithmia.GeneralDataStructures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerTags : Singleton
{
    const bool DEBUG_MODE = true;

    public static MultiValueDictionary<int, string> AllTags = new MultiValueDictionary<int, string>();

    protected override void Awake()
    {
        base.Awake();

        if (DEBUG_MODE)
            Debug.Log("Tags Cleared");

        AllTags.Clear();
    }

    public void AddTag(int ID, string[] tags)
    {
        AllTags.AddRange(ID, tags);

        if (DEBUG_MODE)
            Debug.Log("Added tag: " + tags[0]);
    }

    public static bool CheckTag(GameObject gameObject, string tag)
    {
        Debug.Log("A " + AllTags.ContainsKey(gameObject.GetInstanceID()));

        if (!AllTags.ContainsKey(gameObject.GetInstanceID()))
            return false;

        Debug.Log("B " + AllTags.ContainsValue(gameObject.GetInstanceID(), tag));

        return AllTags.ContainsValue(gameObject.GetInstanceID(), tag);
    }

    public void RemoveTag(int ID)
    {
        AllTags.Remove(ID);
    }

    private void OnDestroy()
    {
        if (DEBUG_MODE)
            Debug.Log("Tags Cleared");

        AllTags.Clear();
    }
}
