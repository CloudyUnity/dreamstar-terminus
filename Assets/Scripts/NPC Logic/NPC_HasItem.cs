using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_HasItem : DialogueLogic
{
    [Header("DIALOGUE : HasItem")]
    [SerializeField] string _item;

    PlayerItems _items;

    protected override void Start()
    {
        base.Start();

        _items = Singleton.Get<PlayerItems>();
    }

    private void Update()
    {
        if (_items.HasItem(_item))
            Dia.ChangeSpokenMessage("HasItem");
    }
}
