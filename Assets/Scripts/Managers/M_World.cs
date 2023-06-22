using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_World : Singleton
{
    public async void LoadScene(string name)
    {
        Get<PlayerMovement>().DisableMovement();

        Get<M_Save>().SaveTheData();

        await Get<M_Camera>().C_Transition(inwards: true);

        ClearMemory();

        SceneManager.LoadScene(name);
    }
}
