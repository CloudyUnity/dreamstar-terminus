using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseMenu : Singleton, ICloseMenu
{
    public static List<ICloseMenu> ChainOfMenus = new List<ICloseMenu>();

    GameObject _menu;
    UIOptionsMenu _options;

    public bool Paused => ChainOfMenus.Count > 0;

    private void Start()
    {
        _options = Get<UIOptionsMenu>();

        ChainOfMenus.Clear();

        _menu = transform.GetChild(0).gameObject;
        _menu.SetActive(false);
    }

    public void OpenPause()
    {
        if (ChainOfMenus.Count == 0)
        {
            Get<PlayerMovement>().DisableMovement();
            ChainOfMenus.Add(this);
        }

        _menu.SetActive(true);
    }

    public void CloseMenu()
    {
        _menu.SetActive(false);
        Get<PlayerMovement>().ReEnableMovement();
        ChainOfMenus.Clear();
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

    public void Log()
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
