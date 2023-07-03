using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseMenu : Singleton, ICloseMenu
{
    public static List<ICloseMenu> ChainOfMenus = new List<ICloseMenu>();

    GameObject _menu;
    UIOptionsMenu _options;
    PlayerMovement _move;

    public bool Paused => ChainOfMenus.Count > 0 && _menu.activeSelf;

    private void Start()
    {
        _options = Get<UIOptionsMenu>();
        _move = Get<PlayerMovement>();

        ChainOfMenus.Clear();

        _menu = transform.GetChild(0).gameObject;
        _menu.SetActive(false);
    }

    public void OpenPause()
    {
        if (_move.MovementDisabled)
            return;

        if (ChainOfMenus.Count == 0)
        {
            _move.DisableMovement();
            ChainOfMenus.Add(this);
        }

        _menu.SetActive(true);
    }

    public void CloseMenu()
    {
        _menu.SetActive(false);
        _move.ReEnableMovement();
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
        Get<M_World>().Restart();
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
