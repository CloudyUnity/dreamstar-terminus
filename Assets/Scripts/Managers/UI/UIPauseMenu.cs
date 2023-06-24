using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseMenu : Singleton, ICloseMenu
{
    public static List<ICloseMenu> ChainOfMenus = new List<ICloseMenu>();

    GameObject _menu;
    UIOptionsMenu _options;

    public bool Paused => _menu.activeSelf;

    private void Start()
    {
        _options = Get<UIOptionsMenu>();

        ChainOfMenus.Clear();

        _menu = transform.GetChild(0).gameObject;
        _menu.SetActive(false);
    }

    public void OpenPause()
    {
        if (Paused)
        {
            CloseMenu();
            return;
        }

        _menu.SetActive(true);
        Get<PlayerMovement>().DisableMovement();
        ChainOfMenus.Add(this);
    }

    public void CloseMenu()
    {
        _menu.SetActive(false);
        Get<PlayerMovement>().ReEnableMovement();
        ChainOfMenus.RemoveAt(ChainOfMenus.Count - 1);
    }

    public void Options()
    {
        _options.OpenOptions();
        _menu.SetActive(false);
    }

    public void Restart()
    {
        CloseMenu();
        Get<M_World>().QuickRestart();
    }

    public void Help()
    {
        // TODO
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void CloseChainMenu()
    {
        if (ChainOfMenus.Count == 0)
        {
            OpenPause();
            return;
        }

        ChainOfMenus[ChainOfMenus.Count - 1].CloseMenu();
    }
}
