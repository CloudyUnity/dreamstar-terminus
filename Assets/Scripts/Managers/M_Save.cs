using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class M_Save : Singleton
{
    public struct SaveData
    {
        public string Version;
        public List<string> Items;
    }

    string _filePath;

    const string VERSION = "v0.0.5";

    private void Start()
    {
        _filePath = Application.persistentDataPath + "/saveData.json";

        if (!File.Exists(_filePath))
        {
            MakeEmptySave();
        }

        LoadData();
    }

    SaveData GetData()
    {
        SaveData data = new SaveData();

        data.Version = VERSION;
        data.Items = new List<string>(Get<PlayerItems>().Items);

        return data;
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

        if (data.Version != VERSION)
        {
            Debug.Log("Incompatible save version!");

            //throw new System.Exception("Incompatible version save data");
        }

        ApplyData(data);
    }

    void ApplyData(SaveData data)
    {
        Get<PlayerItems>().Items = new List<string>(data.Items);

        Debug.Log("Data Applied");
    }

    public void MakeEmptySave()
    {
        SaveData data = new SaveData
        {
            Version = VERSION,
            Items = new List<string>(),
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_filePath, json);
    }
}
