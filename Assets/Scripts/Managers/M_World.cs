using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_World : Singleton
{
    private async void Start()
    {
        Get<PlayerMovement>().DisableMovement();

        RemovedNulls = false;

        await Get<M_Transition>().TransitionAsync(inwards: false);

        Get<PlayerMovement>().ReEnableMovement();
    }

    public async void LoadScene(string name)
    {
        Get<PlayerMovement>().DisableMovement();

        Get<M_Save>().SaveTheData();

        await Get<M_Transition>().TransitionAsync(inwards: true);

        SceneManager.LoadScene(name);
    }
}
