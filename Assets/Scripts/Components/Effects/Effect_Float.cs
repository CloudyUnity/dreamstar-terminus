using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Float : MonoBehaviour
{
    [SerializeField] float _period, _amplitude;
    [SerializeField] bool _worldSpace;
    Vector2 _startPos;

    private void Start()
    {
        _startPos = _worldSpace ? transform.position : transform.localPosition;     
    }

    private void Update()
    {
        Vector2 newPos = _startPos;
        newPos.y += _amplitude * Mathf.Sin(2 * Time.time * Mathf.PI / _period);

        if (_worldSpace)
        {
            transform.position = newPos;
            return;
        }
        transform.localPosition = newPos;
    }
}
