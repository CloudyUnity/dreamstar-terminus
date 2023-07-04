using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_World : Singleton
{
    M_Transition _transition;

    public Vector2 LastEntrance;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        PlayerMovement move = Get<PlayerMovement>();
        _transition = Get<M_Transition>();

        move.DisableMovement();
        await _transition.TransitionAsync(inwards: false);
        move.ReEnableMovement();
    }

    public async void SaveAndLoadScene(string name, Vector2 entrance, Vector2 directionOfMovement)
    {
        LastEntrance = entrance;

        Get<M_Save>().SaveTheData();

        await LoadScene(name, entrance, directionOfMovement);
    }

    public async Task LoadScene(string name, Vector2 entrance, Vector2 directionOfMovement)
    {
        PlayerMovement move = Get<PlayerMovement>();

        // In
        move.DisableMovement();
        await _transition.TransitionAsync(inwards: true);

        Debug.Log("Loading Scene: " + name);
        SceneManager.LoadScene(name);
        Debug.Log("Loaded Scene: " + name);

        // Wait for Awake();
        await Task.Delay(100);

        M_Events.IvkSceneReloaded();

        #region LOAD PLAYER
        move = Get<PlayerMovement>();
        move.DisableMovement();
        
        if (entrance != Vector2.zero)
            move.transform.position = entrance;

        if (directionOfMovement != Vector2.zero)
            move.ActivateSceneChange(directionOfMovement, M_Transition.DURATION);
        #endregion

        Get<M_Camera>().transform.position = new Vector3(move.transform.position.x, move.transform.position.y, -10);

        // Out
        await _transition.TransitionAsync(inwards: false);
        move.ReEnableMovement();
    }

    public async void Restart()
    {
        NewSave();

        await LoadScene("1-1", Vector2.zero, Vector2.zero);
    }

    public void NewSave()
    {
        Get<M_Save>().MakeEmptySave();
        Get<M_Travel>().ClearAllData();
    }
}
