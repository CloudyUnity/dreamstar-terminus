using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransTrigger : MonoBehaviour
{
    [SerializeField] string _sceneChangeName;
    [SerializeField] Vector2 _entrancePos;
    [SerializeField] Vector2 _directionOfMovement;

    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement move = Singleton.Get<PlayerMovement>();

        if (move.MovementDisabled)
            return;
        move.DisableMovement();

        move.ActivateSceneChange(_directionOfMovement);

        Singleton.Get<M_World>().SaveAndLoadScene(_sceneChangeName, _entrancePos, _directionOfMovement);
    }
}
