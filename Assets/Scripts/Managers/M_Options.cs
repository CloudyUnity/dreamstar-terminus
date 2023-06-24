using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class M_Options : Singleton
{
    [System.Serializable]
    public struct OptionData
    {
        public bool ScreenshakeOn;
        public bool PPOn;
        public bool HUDOn;
        public bool PromptsOn;

        public float SFXVolume;
        public float MusicVolume;
    }

    [SerializeField] OptionData _defaultOptionData;

    [ButtonInvoke(nameof(MakeEmptySave))] public bool SetToDefault;

    public OptionData CurrentOptionData;

    string _filePath;

    protected override void Awake()
    {
        base.Awake();

        _filePath = Application.persistentDataPath + "/optionData.json";

        if (!File.Exists(_filePath))
        {
            MakeEmptySave();
        }

        LoadData();
    }

    public void SaveTheData()
    {
        string json = JsonUtility.ToJson(CurrentOptionData);
        File.WriteAllText(_filePath, json);

        Debug.Log("Option Data Saved");
    }

    public void DeleteData()
    {
        File.Delete(_filePath);

        Debug.Log("Option Data Deleted");
    }

    public void LoadData()
    {
        string jsonString = File.ReadAllText(_filePath);
        CurrentOptionData = JsonUtility.FromJson<OptionData>(jsonString);

        Debug.Log("Option Data Loaded");
    }

    public void MakeEmptySave()
    {
        string json = JsonUtility.ToJson(_defaultOptionData);
        File.WriteAllText(_filePath, json);
    }

    public void ChangeOptions(OptionData data)
    {
        if (Time.timeSinceLevelLoad < 0.5f)
            return;

        CurrentOptionData = data;
        SaveTheData();
    }
}
