using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBased : MonoBehaviour
{
    [SerializeField] Vector2 _gridPattern = new Vector2(0.5f, 0.5f);

    [SerializeField] bool _flick;

    private void OnValidate()
    {
        Vector2 pos = transform.position;

        pos.x = Mathf.Round((pos.x - 0.25f) / _gridPattern.x) * _gridPattern.x;
        pos.y = Mathf.Round((pos.y - 0.25f) / _gridPattern.y) * _gridPattern.y;

        pos.x += 0.25f;
        pos.y += 0.25f;

        transform.position = pos;
    }
}
