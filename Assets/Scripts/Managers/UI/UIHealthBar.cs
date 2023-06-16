using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthBar : Singleton
{
    [SerializeField] GameObject _hpImage;
    [SerializeField] Vector2 _firstHpPos;
    [SerializeField] float _hpSpacing;
    [SerializeField] List<GameObject> _hpList = new List<GameObject>();
    public int _hpCountOn;

    PlayerSystems _systems;

    private void Start()
    {
        _systems = Get<PlayerSystems>();

        int hp = _systems.StartingHP;
        _hpCountOn = hp;

        for (int i = 0; i < hp; i++)
        {
            GameObject go = Instantiate(_hpImage, transform);
            go.transform.localPosition = new Vector3(_firstHpPos.x + _hpSpacing * i, _firstHpPos.y);
            _hpList.Add(go);
        }
    }

    private void Update()
    {
        if (_systems.Dead)
        {
            // Shatter last hp or something
            return;
        }

        if (_systems.HP == _hpCountOn)
            return;

        for (int i = 0; i < _systems.HP; i++)
        {
            _hpList[i].SetActive(true);
        }

        for (int i = _systems.HP; i < _hpCountOn; i++)
        {
            _hpList[i].SetActive(false);
        }

        _hpCountOn = _systems.HP;
    }
}
