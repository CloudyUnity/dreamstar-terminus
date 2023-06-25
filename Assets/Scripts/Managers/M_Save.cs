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
        public float TimePassed;
        public int HP;
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

        SaveData data = LoadData();
        ApplyData(data);
    }

    SaveData GetData()
    {
        SaveData data = new SaveData();

        data.Version = VERSION;

        data.Items = new List<string>(Get<PlayerItems>().Items);
        data.TimePassed = Get<M_Time>().TimePassed;
        data.HP = Get<PlayerSystems>().HP;

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

    public SaveData LoadData()
    {
        Debug.Log("Save Data Loaded");

        string jsonString = File.ReadAllText(_filePath);
        SaveData data = JsonUtility.FromJson<SaveData>(jsonString);        

        if (data.Version != VERSION)
        {
            Debug.Log("Incompatible save version!");

            //throw new System.Exception("Incompatible version save data");
        }

        return data;
    }

    void ApplyData(SaveData data)
    {
        Get<PlayerItems>().Items = new List<string>(data.Items);
        Get<PlayerSystems>().HP = data.HP;
        Get<M_Time>().TimePassed = data.TimePassed;

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
