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

    [ButtonInvoke(nameof(MakeEmptySaveOptions))] public bool SetToDefault;

    public OptionData CurrentOptionData;

    public string FilePath => Application.persistentDataPath + "/optionData.json";

    protected override void Awake()
    {
        base.Awake();

        if (!File.Exists(FilePath))
        {
            MakeEmptySaveOptions();
        }

        LoadDataOptions();
    }

    public void SaveTheDataOptions()
    {
        string json = JsonUtility.ToJson(CurrentOptionData);
        File.WriteAllText(FilePath, json);

        Debug.Log("Option Data Saved");
    }

    public void DeleteDataOptions()
    {
        File.Delete(FilePath);

        Debug.Log("Option Data Deleted");
    }

    public void LoadDataOptions()
    {
        string jsonString = File.ReadAllText(FilePath);
        CurrentOptionData = JsonUtility.FromJson<OptionData>(jsonString);

        Debug.Log("Option Data Loaded");
    }

    public void MakeEmptySaveOptions()
    {
        string json = JsonUtility.ToJson(_defaultOptionData);
        File.WriteAllText(FilePath, json);
    }

    public void ChangeOptions(OptionData data)
    {
        if (Time.timeSinceLevelLoad < 0.5f)
            return;

        CurrentOptionData = data;
        SaveTheDataOptions();
    }
}
