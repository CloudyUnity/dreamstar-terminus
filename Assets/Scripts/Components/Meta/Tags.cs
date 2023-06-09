using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour
{
    [SerializeField] string[] _tags;

    M_Tags _tagManager;

    private void Start()
    {
        if (_tags.Length == 0)
        {
            Debug.Log("No tags detected on gameobject: " + name);
            enabled = false;
            return;
        }

        _tagManager = Singleton.Get<M_Tags>();

        _tagManager.AddTag(gameObject.GetInstanceID(), _tags);
    }

    private void OnDestroy()
    {
        if (_tags.Length == 0)
            return;

        _tagManager.RemoveTag(gameObject.GetInstanceID());
    }
}
