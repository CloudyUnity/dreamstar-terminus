using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerWorld : Singleton
{
    public void LoadScene(string name)
    {
        Get<ManagerSave>().SaveTheData();

        // Transiion

        ClearMemory();

        SceneManager.LoadScene(name);
    }
}
