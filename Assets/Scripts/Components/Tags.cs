using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tags : MonoBehaviour
{
    [SerializeField] string[] _tags;

    ManagerTags _tagManager;

    private void Start()
    {
        if (_tags.Length == 0)
            return;

        _tagManager = Singleton.Get<ManagerTags>();

        _tagManager.AddTag(gameObject.GetInstanceID(), _tags);
    }

    private void OnDestroy()
    {
        _tagManager.RemoveTag(gameObject.GetInstanceID());
    }
}
