using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelManager : Singleton
{
    [HideInInspector] public List<WorldHistory> WorldHisList = new List<WorldHistory>();
    [HideInInspector] public List<Traveller> CurrentTravs = new List<Traveller>();

    [SerializeField] float _timeCreationSpeed;
    [SerializeField] int _historyMaxLength;
    float _timeCreationTimer;

    WorldHistory _present;

    private void Start()
    {
        ClearAll();

        CreateNewTime();        
    }

    private void Update()
    {
        _timeCreationTimer += Time.deltaTime;
        
        if (_timeCreationTimer >= _timeCreationSpeed)
        {
            CreateNewTime();
            _timeCreationTimer = 0;            
        }
    }

    void AddPosition(Traveller trav)
    {
        if (_present.TravPos.ContainsKey(trav))
            return;

        _present.TravPos.Add(trav, trav.transform.position);
    }

    void ClearAll()
    {
        WorldHisList.Clear();
        CurrentTravs.Clear();
    }

    void CreateNewTime()
    {
        _present = new WorldHistory();
        WorldHisList.Add(_present);

        if (WorldHisList.Count > _historyMaxLength)
            WorldHisList.RemoveAt(0);

        AddAllTravellersPos();
        PlayerMovement m = Get<PlayerMovement>();
        _present.PlayerPos = Get<PlayerMovement>().transform.position;

        Debug.Log("New Time Created with playPos: " + _present.PlayerPos);
    }

    void AddAllTravellersPos()
    {
        foreach (Traveller trav in CurrentTravs)
        {
            AddPosition(trav);
        }
    }

    public void RollBackTime(float seconds)
    {
        int index = Mathf.Clamp(Mathf.RoundToInt(seconds / _timeCreationSpeed), 0, _historyMaxLength - 1);

        WorldHistory time = WorldHisList[(WorldHisList.Count - 1) - index];

        foreach (Traveller trav in CurrentTravs)
        {
            trav.transform.position = time.TravPos[trav];
        }

        Get<PlayerMovement>().transform.position = time.PlayerPos;
    }
}

public class WorldHistory
{
    public Dictionary<Traveller, Vector2> TravPos;
    public Vector2 PlayerPos;
}
