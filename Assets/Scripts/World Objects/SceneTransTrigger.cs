using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransTrigger : MonoBehaviour
{
    [SerializeField] string _sceneChangeName;
    [SerializeField] Vector2 _directionOfMovement;

    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement move = Singleton.Get<PlayerMovement>();

        if (move.MovementDisabled)
            return;
        move.DisableMovement();

        move.ActivateSceneChange(_directionOfMovement);

        await Singleton.Get<M_World>().LoadScene(_sceneChangeName);
    }
}
