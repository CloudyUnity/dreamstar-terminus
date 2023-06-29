using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_World : Singleton
{
    PlayerMovement _move;
    M_Transition _transition;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        _move = Get<PlayerMovement>();
        _transition = Get<M_Transition>();

        _move.DisableMovement();
        await _transition.TransitionAsync(inwards: false);
        _move.ReEnableMovement();
    }

    public async Task LoadScene(string name)
    {
        Get<M_Save>().SaveTheData();

        _move.DisableMovement();
        await _transition.TransitionAsync(inwards: true);

        // TODO: Loading screen/bar/wheel/etc with LoadSceneAsync
        SceneManager.LoadScene(name);

        Singleton.RemoveNulls();

        await Task.Delay(100);

        M_Events.IvkSceneReloaded();

        _move = Get<PlayerMovement>();

        _move.DisableMovement();
        await _transition.TransitionAsync(inwards: false);
        _move.ReEnableMovement();
    }

    public async void QuickRestart()
    {
        await LoadScene("1-1");
    }

    public void NewSave()
    {
        Get<M_Save>().MakeEmptySave();
        Get<M_Travel>().ClearAllData();
        QuickRestart();
    }
}
