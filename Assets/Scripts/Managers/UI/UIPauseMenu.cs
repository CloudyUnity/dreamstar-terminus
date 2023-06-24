using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseMenu : Singleton
{
    GameObject _menu;

    public bool Paused => _menu.activeSelf;

    private void Start()
    {
        _menu = transform.GetChild(0).gameObject;
        _menu.SetActive(false);
    }

    public void OpenPause()
    {
        if (Paused)
        {
            Resume();
            return;
        }

        _menu.SetActive(true);
        Get<PlayerMovement>().DisableMovement();
    }

    public void Resume()
    {
        _menu.SetActive(false);
        Get<PlayerMovement>().ReEnableMovement();
    }

    public void Options()
    {
        // TODO
    }

    public void Restart()
    {
        Resume();
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
}
