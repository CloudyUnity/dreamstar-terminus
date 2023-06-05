using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelManager : Singleton
{
    public List<WorldHistory> Timeline = new List<WorldHistory>();
    public List<Traveller> CurrentTravs = new List<Traveller>();

    [SerializeField] float _timeCreationSpeed;
    [SerializeField] int _historyMaxLength;
    [SerializeField] float _cooldown;
    float _timeCreationTimer, _cooldownTimer;

    WorldHistory _present;

    private void Start()
    {
        CreateNewTime();        
    }

    private void Update()
    {
        _timeCreationTimer += Time.deltaTime;
        _cooldownTimer += Time.deltaTime;
        
        if (_timeCreationTimer >= _timeCreationSpeed)
        {
            CreateNewTime();
            _timeCreationTimer = 0;            
        }
    }

    void AddPosition(Traveller trav)
    {
        if (_present.TravPos.ContainsKey(trav.ID))
            return;

        _present.TravPos.Add(trav.ID, trav.transform.position);
    }

    void ClearAll()
    {
        Timeline.Clear();
        CurrentTravs.Clear();
    }

    void CreateNewTime()
    {
        #region CREATE PRESENT
        _present = new WorldHistory();
        _present.TravPos = new Dictionary<int, Vector2>();
        Timeline.Add(_present);
        #endregion

        if (Timeline.Count > _historyMaxLength)
            Timeline.RemoveAt(0);

        StoreTimeInfo();       

        Debug.Log("New Time Created with playPos: " + _present.PlayerPos);
    }

    void StoreTimeInfo()
    {
        foreach (Traveller trav in CurrentTravs)
        {
            AddPosition(trav);
        }

        _present.PlayerPos = Get<PlayerMovement>().transform.position;
    }

    public void RollBackTime(float seconds)
    {
        #region COOLDOWN
        if (_cooldownTimer < _cooldown)
        {
            Debug.Log("TIME TRAVEL NOT READY");
            return;
        }
        _cooldownTimer = 0;
        #endregion

        int index = Mathf.Clamp(Mathf.RoundToInt(seconds / _timeCreationSpeed), 0, _historyMaxLength - 1);

        WorldHistory time = Timeline[(Timeline.Count - 1) - index];

        ApplyTimeInfo(time);

        // Delete original timeline
        for (int i = 0; i < index; i++)
            Timeline.RemoveAt(Timeline.Count - 1);      
    }

    void ApplyTimeInfo(WorldHistory time)
    {
        foreach (Traveller trav in CurrentTravs)
        {
            trav.transform.position = time.TravPos[trav.ID];
        }

        Get<PlayerMovement>().transform.position = time.PlayerPos;
    }
}

public class WorldHistory
{
    public Dictionary<int, Vector2> TravPos;
    public Vector2 PlayerPos;
}
