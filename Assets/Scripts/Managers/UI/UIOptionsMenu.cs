using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOptionsMenu : Singleton, ICloseMenu
{
    GameObject _menu;

    UIPauseMenu _pause;
    M_Options _optionsManager;
    UIKeybindsMenu _keybinds;

    [SerializeField] Toggle _screenshake, _pp, _hud, _prompts;
    [SerializeField] Slider _sfx, _music;

    private void Start()
    {
        _pause = Get<UIPauseMenu>();
        _optionsManager = Get<M_Options>();
        _keybinds = Get<UIKeybindsMenu>();

        _menu = transform.GetChild(0).gameObject;
        _menu.SetActive(false);

        SetMenuToOptions();
    }

    void SetMenuToOptions()
    {
        _screenshake.isOn = _optionsManager.CurrentOptionData.ScreenshakeOn;
        _pp.isOn = _optionsManager.CurrentOptionData.PPOn;
        _hud.isOn = _optionsManager.CurrentOptionData.HUDOn;
        _prompts.isOn = _optionsManager.CurrentOptionData.PromptsOn;

        _sfx.value = _optionsManager.CurrentOptionData.SFXVolume;
        _music.value = _optionsManager.CurrentOptionData.MusicVolume;
    }

    public void OpenOptions()
    {
        if (UIPauseMenu.ChainOfMenus.Count == 1)
        {
            UIPauseMenu.ChainOfMenus.Add(this);
        }

        _menu.SetActive(true);        
    }

    public void CloseMenu()
    {
        _menu.SetActive(false);
        _pause.OpenPause();
        UIPauseMenu.ChainOfMenus.RemoveAt(UIPauseMenu.ChainOfMenus.Count - 1);
    }

    public void KeyBinds()
    {
        _keybinds.OpenKeyBinds();
        _menu.SetActive(false);
    }

    public void ToggleScreenshake(bool toggle)
    {
        if (Time.timeSinceLevelLoad < 0.5f)
            return;

        M_Options.OptionData data = _optionsManager.CurrentOptionData;
        data.ScreenshakeOn = toggle;
        _optionsManager.ChangeOptions(data);
    }

    public void TogglePP(bool toggle)
    {
        if (Time.timeSinceLevelLoad < 0.5f)
            return;

        M_Options.OptionData data = _optionsManager.CurrentOptionData;
        data.PPOn = toggle;
        _optionsManager.ChangeOptions(data);
    }

    public void ToggleHUD(bool toggle)
    {
        if (Time.timeSinceLevelLoad < 0.5f)
            return;

        M_Options.OptionData data = _optionsManager.CurrentOptionData;
        data.HUDOn = toggle;
        _optionsManager.ChangeOptions(data);
    }

    public void TogglePrompts(bool toggle)
    {
        if (Time.timeSinceLevelLoad < 0.5f)
            return;

        M_Options.OptionData data = _optionsManager.CurrentOptionData;
        data.PromptsOn = toggle;
        _optionsManager.ChangeOptions(data);
    }

    public void SliderSFX(float value)
    {
        if (Time.timeSinceLevelLoad < 0.5f)
            return;

        M_Options.OptionData data = _optionsManager.CurrentOptionData;
        data.SFXVolume = value;
        _optionsManager.ChangeOptions(data);
    }

    public void SliderMusic(float value)
    {
        if (Time.timeSinceLevelLoad < 0.5f)
            return;

        M_Options.OptionData data = _optionsManager.CurrentOptionData;
        data.MusicVolume = value;
        _optionsManager.ChangeOptions(data);
    }
}
