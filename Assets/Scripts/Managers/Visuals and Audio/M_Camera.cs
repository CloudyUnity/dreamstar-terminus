using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Camera : Singleton
{
    PlayerMovement _player;
    M_Transition _transition;
    M_Options _options;

    Camera _cam;
    bool _shaking { get { return _curMag != 0; } }
    float _curMag;
    float _camSpeed;
    Vector2? _lock = null;

    [SerializeField] List<FocalPoint> _focalPoints = new List<FocalPoint>();
    [Space(5)]
    [SerializeField] Vector3 _offset;
    [Space(5)]
    [SerializeField] float _smallBiasRange = 3;
    [SerializeField] float _bigBiasRange = 5;
    [SerializeField] float _closeBonusRange = 2;
    [SerializeField] float _focalPointMag = 0.3f;
    [SerializeField] float _targetSpeed = 3.5f;
    [SerializeField] float _targetSpeedFocal = 0.9f;
    [Space(5)]
    [SerializeField] float _playerFarMult;
    [SerializeField] float _playerFarDistance;

    private void Start()
    {
        _player = Get<PlayerMovement>();
        _transition = Get<M_Transition>();
        _options = Get<M_Options>();
        _cam = GetComponent<Camera>();        

        SetAspect();        
    }

    private void Update()
    {
        SetAspect();
    }

    private void LateUpdate()
    {
        if (_shaking || _player == null || _transition.Transitioning)
            return;

        transform.position = GetNextPosition();
    }
    
    public void AddFocalPoint(FocalPoint fp) => _focalPoints.Add(fp);

    Vector3 GetNextPosition()
    {       
        Vector3 pos = GetTargetPos(out bool noFocalPoints);
        pos.z = -10;
        pos += _offset;

        float targetSpeed = noFocalPoints ? _targetSpeed : _targetSpeedFocal;

        float dis = Vector3.Distance(pos, transform.position);
        if (dis >= _playerFarDistance)
            targetSpeed *= dis - 3;

        if (!_player.Grounded && !_player.Walled)
            targetSpeed *= _playerFarMult;

        if (targetSpeed < _camSpeed)
            _camSpeed = targetSpeed;

        _camSpeed = Mathf.Lerp(_camSpeed, targetSpeed, Time.deltaTime * 0.55f);

        return Vector3.Lerp(transform.position, pos, Time.deltaTime * _camSpeed);
    }

    Vector2 GetTargetPos(out bool noFocalPoints)
    {
        noFocalPoints = true;

        if (_lock != null)
            return _lock.GetValueOrDefault();

        #region FocalPoints
        List<Vector3> validFocalPoints = new List<Vector3>();
        for (int i = _focalPoints.Count - 1; i >= 0; i--)
        {
            if (_focalPoints[i] == null)
            {
                _focalPoints.RemoveAt(i);
                continue;
            }

            if (_focalPoints[i].Bias <= 1 && Vector3.Distance(_player.transform.position, _focalPoints[i].transform.position) > _smallBiasRange)
                continue;

            if (_focalPoints[i].Bias >= 2 && Vector3.Distance(_player.transform.position, _focalPoints[i].transform.position) > _bigBiasRange)
                continue;

            if (Vector3.Distance(_player.transform.position, _focalPoints[i].transform.position) < _closeBonusRange)
                validFocalPoints.Add(_focalPoints[i].transform.position);

            for (int j = 0; j < _focalPoints[i].Bias; j++)
                validFocalPoints.Add(_focalPoints[i].transform.position);
        }
        #endregion

        Vector3 averageFocalPoint = AverageOfPoints(validFocalPoints, out noFocalPoints);
        Vector3 dir = Vector3.zero;

        if (!noFocalPoints)
        {
            dir = averageFocalPoint - _player.transform.position;
        }

        return _player.transform.position + (dir * _focalPointMag);
    }

    Vector3 AverageOfPoints(List<Vector3> points, out bool noFocalPoints)
    {        
        if (points.Count == 0)
        {
            noFocalPoints = true;
            return Vector3.zero;
        }

        noFocalPoints = false;

        Vector3 average = Vector3.zero;
        foreach (Vector3 point in points)
        {
            average += point;
        }

        return average / points.Count;
    }

    public void ScreenShake(float mag, float dur)
    {
        if (mag <= _curMag)
            return;

        if (!_options.CurrentOptionData.ScreenshakeOn)
            return;

        StopAllCoroutines();
        StartCoroutine(C_ScreenShake(mag, dur));
    }

    IEnumerator C_ScreenShake(float mag, float dur)
    {
        _curMag = mag;

        float elapsed = 0;

        float seed = Random.Range(0, 1000f);

        while (elapsed < dur)
        {
            Vector3 pos = GetNextPosition();

            pos.x += (Mathf.PerlinNoise(seed, elapsed * 4) - 0.5f) * mag;
            pos.y += (Mathf.PerlinNoise(seed + 5, elapsed * 4 + 1) - 0.5f) * mag;
            pos.z = -10;

            float rotation = (Mathf.PerlinNoise(seed + 20, elapsed * 4 + 2) - 0.5f) * mag;

            transform.position = pos;

            transform.rotation = Quaternion.Euler(0, 0, rotation);

            elapsed += Time.deltaTime;
            yield return null;
        }
        _curMag = 0;
    }

    void SetAspect()
    {
        float targetaspect = 16.0f / 9.0f;

        float windowaspect = (float)Screen.width / Screen.height;

        float scaleheight = windowaspect / targetaspect;

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = _cam.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            _cam.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = _cam.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            _cam.rect = rect;
        }
    }

    public void SetLock(Vector2 pos) => _lock = pos;

    public void RemoveLock() => _lock = null;
}