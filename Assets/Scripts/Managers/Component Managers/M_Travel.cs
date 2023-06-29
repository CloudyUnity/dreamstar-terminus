using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_Travel : Singleton
{
    public List<TimePoint> Timeline = new List<TimePoint>();
    public List<Traveller> CurrentTravs = new List<Traveller>();

    [SerializeField] float _timeCreationSpeed;
    [SerializeField] int _historyMaxLength;

    [SerializeField] GameObject _corpsePrefab;
    [SerializeField] bool _paradoxesOn;

    public bool OnCooldown => _cooldown > 0;
    float _cooldown;

    TimePoint _present;
    M_Time _timeManager;
    PlayerMovement _move;

    #region ON LOAD
    private void OnEnable()
    {
        M_Events.SceneReloaded += GetReferences;
        M_Events.SceneReloaded += ApplyNearestTimePoint;
    }

    private void OnDisable()
    {
        M_Events.SceneReloaded -= GetReferences;
        M_Events.SceneReloaded -= ApplyNearestTimePoint;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        GetReferences();
    }

    void GetReferences()
    {
        _timeManager = Get<M_Time>();
        _move = Get<PlayerMovement>();
    }

    void ApplyNearestTimePoint() => ApplyTimeInfo(Timeline[Timeline.Count - 1]);
    #endregion

    #region CREATING TIME POINTS
    private void Update()
    {
        if (_move == null || _move.MovementDisabled || _timeManager == null)
            return;

        _cooldown -= Time.deltaTime;

        float timeSinceLastPoint = _timeManager.TimePassed - (Timeline.Count * _timeCreationSpeed);
        
        if (timeSinceLastPoint >= _timeCreationSpeed)
            CreateNewTimePoint();
    }

    void CreateNewTimePoint()
    {
        _present = new TimePoint();
        _present.TravPos = new Dictionary<int, Vector2>();

        Timeline.Add(_present);

        if (Timeline.Count > _historyMaxLength)
            Timeline.RemoveAt(0);

        StoreTimeInfoInPoint();       
    }

    void StoreTimeInfoInPoint()
    {
        foreach (Traveller trav in CurrentTravs)
        {
            AddPosToCurrentTimePoint(trav);
        }

        _present.TimePassed = _timeManager.TimePassed;

        _present.PlayerPos = _move.transform.position;

        _present.SceneName = SceneManager.GetActiveScene().name;
    }

    void AddPosToCurrentTimePoint(Traveller trav)
    {
        if (_present.TravPos.ContainsKey(trav.gameObject.GetInstanceID()))
            return;

        _present.TravPos.Add(trav.gameObject.GetInstanceID(), trav.transform.position);
    }
    #endregion

    #region CHANGING TIME
    public async Task TimeTravelBackAsync(float seconds)
    {
        if (OnCooldown)
        {
            Debug.Log("TIME TRAVEL NOT READY");
            return;
        }
        _cooldown = seconds;

        _timeManager.ReduceTime(seconds);

        TimePoint point = FindPreviousTimePoint(seconds, out int backAmount, out int pointIndex);

        // Delete original timeline      
        Timeline.RemoveRange(pointIndex, backAmount);

        ApplyTimeInfo(point);

        // TODO: Anims, effects, etc
        // Custom reversed transition?
        await Task.Delay(1);
    }

    public TimePoint FindPreviousTimePoint(float seconds, out int backAmount, out int pointIndex)
    {
        backAmount = Mathf.Clamp(Mathf.RoundToInt(seconds / _timeCreationSpeed), 0, Timeline.Count - 1);
        pointIndex = Timeline.Count - 1 - backAmount;

        return Timeline[pointIndex]; 
    }

    void ApplyTimeInfo(TimePoint time)
    {
        for (int i = CurrentTravs.Count - 1; i >= 0; i--)
        {
            Traveller trav = CurrentTravs[i];

            bool pointContainsTrav = time.TravPos.ContainsKey(trav.gameObject.GetInstanceID());
            if (!pointContainsTrav)
            {
                CurrentTravs.Remove(trav);
                Destroy(trav.gameObject);
                continue;
            }

            trav.transform.position = time.TravPos[trav.gameObject.GetInstanceID()];
            trav.CheckDeath();
        }

        if (_paradoxesOn)
            Instantiate(_corpsePrefab, time.PlayerPos, Quaternion.identity);        
    }
    #endregion

    public void ClearAllData()
    {
        Timeline.Clear();
        CurrentTravs.Clear();
        _cooldown = 0;
    }
}

public class TimePoint
{
    public float TimePassed;
    public string SceneName;
    public Dictionary<int, Vector2> TravPos;
    public Vector2 PlayerPos;
}
