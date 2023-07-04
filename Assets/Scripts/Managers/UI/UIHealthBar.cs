using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : Singleton
{
    [SerializeField] Image _hpImage;
    [SerializeField] Sprite _full, _empty;
    [SerializeField] Vector2 _firstHpPos;
    [SerializeField] float _hpSpacing;
    [SerializeField] List<Image> _hpList = new List<Image>();
    public int _hpCountOn;

    PlayerSystems _systems;

    private void Start()
    {
        _systems = Get<PlayerSystems>();

        int hp = _systems.StartingHP;
        _hpCountOn = hp;

        for (int i = 0; i < hp; i++)
        {
            Image image = Instantiate(_hpImage, transform);
            image.transform.localPosition = new Vector3(_firstHpPos.x + _hpSpacing * i, _firstHpPos.y);
            _hpList.Add(image);
        }
    }

    private void Update()
    {
        if (_systems.Dead)
        {
            // Shatter last hp or something
            //return;
        }

        if (_systems.HP == _hpCountOn)
            return;

        for (int i = 0; i < _systems.HP; i++)
        {
            _hpList[i].sprite = _full;
        }

        for (int i = _systems.HP; i < _hpCountOn; i++)
        {
            _hpList[i].sprite = _empty;
        }

        _hpCountOn = _systems.HP;
    }
}
