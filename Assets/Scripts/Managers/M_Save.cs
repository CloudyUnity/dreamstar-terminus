using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class M_Save : Singleton
{
    public struct SaveData
    {
        public string Version;
        public List<string> Items;
        public float TimePassed;
        public int HP;

        public string CurrentScene;
        public Vector2 LastEntrance;

        public bool ActiveSave;
    }

    public string FilePath => Application.persistentDataPath + "/saveData.json";

    const string VERSION = "v0.0.5";

    [ButtonInvoke(nameof(MakeEmptySave))] public bool DeleteSaveData;

    private void Start()
    {
        if (!File.Exists(FilePath))
        {
            MakeEmptySave();
        }

        SaveData data = LoadData();

        if (!data.ActiveSave)
            return;

        ApplyData(data);
    }

    SaveData GetData()
    {
        SaveData data = new SaveData();

        data.Version = VERSION;

        data.Items = new List<string>(Get<PlayerItems>().Items);
        data.TimePassed = Get<M_Time>().TimePassed;
        data.HP = Get<PlayerSystems>().HP;

        data.CurrentScene = SceneManager.GetActiveScene().name;
        data.LastEntrance = Get<M_World>().LastEntrance;

        data.ActiveSave = true;

        return data;
    }

    public void SaveTheData()
    {
        SaveData data = GetData();

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(FilePath, json);

        Debug.Log("Data Saved");
    }

    public void DeleteData()
    {       
        File.Delete(FilePath);

        Debug.Log("Data Deleted");
    }

    public SaveData LoadData()
    {
        Debug.Log("Save Data Loaded");

        string jsonString = File.ReadAllText(FilePath);
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

            CurrentScene = "1-1",
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(FilePath, json);
    }
}
