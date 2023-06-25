using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_World : Singleton
{
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        Get<PlayerMovement>().DisableMovement();
        await Get<M_Transition>().TransitionAsync(inwards: false);
        Get<PlayerMovement>().ReEnableMovement();
    }

    public async Task LoadScene(string name)
    {
        Get<M_Save>().SaveTheData();

        Get<PlayerMovement>().DisableMovement();
        await Get<M_Transition>().TransitionAsync(inwards: true);

        // TODO: Loading screen/bar/wheel/etc with LoadSceneAsync
        SceneManager.LoadScene(name);
        await Task.Delay(100);

        M_Events.IvkSceneReloaded();

        Get<PlayerMovement>().DisableMovement();
        await Get<M_Transition>().TransitionAsync(inwards: false);
        Get<PlayerMovement>().ReEnableMovement();
    }

    public async void QuickRestart()
    {
        await LoadScene("Block-Out-Test");
    }

    public void NewSave()
    {
        Get<M_Save>().MakeEmptySave();
        Get<M_Travel>().ClearAllData();
        QuickRestart();
    }
}
