using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Travel : Singleton
{
    public List<WorldHistory> Timeline = new List<WorldHistory>();
    public List<Traveller> CurrentTravs = new List<Traveller>();

    [SerializeField] float _timeCreationSpeed;
    [SerializeField] int _historyMaxLength;
    float _timeCreationTimer;

    [SerializeField] GameObject _corpsePrefab;

    public bool OnCooldown => _cooldown > 0;
    float _cooldown;

    WorldHistory _present;
    M_Time _time;
    PlayerMovement _move;

    private void Start()
    {
        _time = Get<M_Time>();
        _move = Get<PlayerMovement>();

        CreateNewTime();   
    }

    private void Update()
    {
        _timeCreationTimer += Time.deltaTime;
        _cooldown -= Time.deltaTime;
        
        if (_timeCreationTimer >= _timeCreationSpeed)
        {
            CreateNewTime();
            _timeCreationTimer = 0;            
        }
    }

    void AddPosition(Traveller trav)
    {
        if (_present.TravPos.ContainsKey(trav.gameObject.GetInstanceID()))
            return;

        _present.TravPos.Add(trav.gameObject.GetInstanceID(), trav.transform.position);
    }

    void CreateNewTime()
    {
        _present = new WorldHistory();
        _present.TravPos = new Dictionary<int, Vector2>();
        Timeline.Add(_present);

        if (Timeline.Count > _historyMaxLength)
            Timeline.RemoveAt(0);

        StoreTimeInfo();       

        //Debug.Log("New Time Created with playPos: " + _present.PlayerPos);
    }

    void StoreTimeInfo()
    {
        foreach (Traveller trav in CurrentTravs)
        {
            AddPosition(trav);
        }

        _present.PlayerPos = _move.transform.position;
    }

    public void RollBackTime(float seconds)
    {
        if (OnCooldown)
        {
            Debug.Log("TIME TRAVEL NOT READY");
            return;
        }

        _timeCreationTimer = 0;

        int index = Mathf.Clamp(Mathf.RoundToInt(seconds / _timeCreationSpeed), 0, Timeline.Count - 1);

        WorldHistory time = Timeline[Timeline.Count - 1 - index];

        ApplyTimeInfo(time);
        _time.TimeTravelled(seconds);

        // Delete original timeline      
        Timeline.RemoveRange(Timeline.Count - 1 - index, index);        

        _cooldown = seconds;
    }

    void ApplyTimeInfo(WorldHistory time)
    {
        for (int i = CurrentTravs.Count - 1; i >= 0; i--)
        {
            Traveller trav = CurrentTravs[i];
            if (!time.TravPos.ContainsKey(trav.gameObject.GetInstanceID()))
            {
                CurrentTravs.Remove(trav);
                Destroy(trav.gameObject);
                continue;
            }

            trav.transform.position = time.TravPos[trav.gameObject.GetInstanceID()];
            trav.CheckDeath();
        }

        Instantiate(_corpsePrefab, time.PlayerPos, Quaternion.identity);        
    }
}

public class WorldHistory
{
    public Dictionary<int, Vector2> TravPos;
    public Vector2 PlayerPos;
}
