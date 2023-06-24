using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIKeybindsMenu : Singleton, ICloseMenu
{
    GameObject _menu;

    UIOptionsMenu _options;

    private void Start()
    {
        _options = Get<UIOptionsMenu>();

        _menu = transform.GetChild(0).gameObject;
        _menu.SetActive(false);
    }

    public void OpenKeyBinds()
    {
        _menu.SetActive(true);
        UIPauseMenu.ChainOfMenus.Add(this);
    }

    public void CloseMenu()
    {
        _menu.SetActive(false);
        _options.OpenOptions();
        UIPauseMenu.ChainOfMenus.RemoveAt(UIPauseMenu.ChainOfMenus.Count - 1);
    }
}
