using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_World : Singleton
{
    public void LoadScene(string name)
    {
        Get<M_Save>().SaveTheData();

        // Transiion

        ClearMemory();

        SceneManager.LoadScene(name);
    }
}
