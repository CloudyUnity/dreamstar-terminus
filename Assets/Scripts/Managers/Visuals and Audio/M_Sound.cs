using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Sound : Singleton
{
    [System.Serializable]
    public struct SFX : System.IComparable<SFX>
    {
        public string Name;
        public AudioClip[] Clips;

        public int CompareTo(SFX other)
        {
            return string.Compare(Name, other.Name);
        }
    }  

    [SerializeField] List<SFX> _allSFX = new List<SFX>();
    [ButtonInvoke(nameof(SortList))] public bool SortTheList;

    [SerializeField] AudioSource _sourceMain;
    [SerializeField] AudioSource _sourceAlt;
    [SerializeField] AudioSource _musicPlayer;
    [SerializeField] AudioSource _musicPaused;
    [SerializeField] AudioSource _ambience;

    AudioSource _currentSource;

    bool _canChangeMusic;
    bool _pausedLastFrame;

    string _lastPlayed;
    float _timeSincePlay;

    bool _startedSong;
    bool _changingSpeed;

    float _musicVolume;
    float _sfxVolume;

    private void SortList()
    {
        _allSFX.Sort();
    }

    private void OnEnable()
    {
        M_Events.ReassignSettings += AssignSettings;
    }

    private void OnDisable()
    {
        M_Events.ReassignSettings -= AssignSettings;
    }

    private void Start()
    {
        _currentSource = _musicPlayer;
        AssignSettings();
    }

    void AssignSettings()
    {
        _sfxVolume = 1;
        _musicVolume = 1;
    }

    private void Update()
    {
        //StartCoroutine(C_RaiseMusic());

        _timeSincePlay += Time.deltaTime;

        //if (V_HUDManager.Instance.IsPaused && !_pausedLastFrame)
        //{
        //    _currentSource.Pause();
        //    _musicPaused.Play();
        //}
        //else if (!V_HUDManager.Instance.IsPaused && _pausedLastFrame)
        //{
        //    if (!_outroDone)
        //        _currentSource.Play();
        //    _musicPaused.Pause();
        //}

        //_pausedLastFrame = V_HUDManager.Instance.IsPaused
    }

    public void PlaySFX(string name)
    {
        //Debug.Log("SFX: " + name);

        if (_sfxVolume <= 0)
            return;

        if (name == _lastPlayed && _timeSincePlay < 0.1f)
            return;

        // Could use binary search here, list is sorted
        foreach (SFX sfx in _allSFX)
        {
            if (sfx.Name == name)
            {
                AudioClip clip = sfx.Clips[Random.Range(0, sfx.Clips.Length)];
                AudioSource source = GetSource(name);
                source.pitch = GetPitch(name);
                source.PlayOneShot(clip);

                _lastPlayed = name;
                _timeSincePlay = 0;
                return;
            }
        }

        Debug.Log("SFX " + name + " is not found");
    }

    float GetPitch(string name)
    {
        return 1;
    }

    AudioSource GetSource(string name)
    {
        if (name.Is("example"))
            return _sourceAlt;

        return _sourceMain;
    }

    IEnumerator C_RaiseMusic()
    {
        float elapsed = 0;
        float dur = 2f;

        _musicPlayer.Play();
        _startedSong = true;

        while (elapsed < dur)
        {
            _musicPlayer.volume = _musicVolume * Mathf.Lerp(0, 1, elapsed / dur);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _canChangeMusic = true;
    }

    void LowerMusicVolume() => StartCoroutine(C_LowerMusic());

    IEnumerator C_LowerMusic()
    {
        _canChangeMusic = false;

        float elapsed = 0;
        float dur = 0.5f;

        while (elapsed < dur)
        {
            //_musicPlayer.volume = A_OptionsManager.Instance.Current.MusicVolume * Mathf.Lerp(1, 0, elapsed / dur);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _musicPlayer.volume = 0;
    }
}
