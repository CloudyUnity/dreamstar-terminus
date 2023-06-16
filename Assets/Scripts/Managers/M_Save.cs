using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class M_Save : Singleton
{
    public struct SaveData
    {
        public string Version;
    }

    string _filePath;

    const string VERSION = "v0.0.3";

    private void Start()
    {
        _filePath = Application.persistentDataPath + "/saveData.json";

        if (!File.Exists(_filePath))
            return;

        LoadData();
    }

    SaveData GetData()
    {
        return default(SaveData);
    }

    public void SaveTheData()
    {
        SaveData data = GetData();

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_filePath, json);

        Debug.Log("Data Saved");
    }

    public void DeleteData()
    {       
        File.Delete(_filePath);

        Debug.Log("Data Deleted");
    }

    public void LoadData()
    {
        string jsonString = File.ReadAllText(_filePath);
        SaveData data = JsonUtility.FromJson<SaveData>(jsonString);

        Debug.Log("Save Data Loaded");

        //if (data.Version != VERSION)
        //    throw new System.Exception("Incompatible version save data");

        ApplyData(data);
    }

    void ApplyData(SaveData data)
    {
        Debug.Log("Data Applied");
    }

    public void MakeEmptySave()
    {
        SaveData data = new SaveData
        {
            Version = VERSION,
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_filePath, json);
    }
}
