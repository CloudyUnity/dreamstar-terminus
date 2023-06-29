using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLock : MonoBehaviour
{
    M_Camera _cam;

    private void Start()
    {
        _cam = Singleton.Get<M_Camera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _cam.SetLock(transform.position);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _cam.RemoveLock();
    }

    private void OnDrawGizmosSelected()
    {
        // Without offset
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector2(12.5f, 7f));

        // With offset
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(0, 1), new Vector2(12.5f, 7f));
    }
}
