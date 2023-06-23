using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItems : Singleton
{
    public List<string> Items = new List<string>();

    public void AddItem(string item) => Items.Add(item);

    public bool HasItem(string item) => Items.Contains(item);

    public bool TryUseItem(string item) => Items.Remove(item);
}
