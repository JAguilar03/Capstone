using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//From tutorial: https://www.youtube.com/watch?v=Wu4SGitck7M
//"JSONUtility - A Better Way to Save Persistent Data in Unity" by Game Dev By Kaupenjoe
public class SaveDataManager : MonoBehaviour
{
    private SaveData saveData;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private WeaponController weapon;

    // Start is called before the first frame update
    void Start()
    {
        saveData = SaveData.Instance;
    }

    public void SaveToFile()
    {
        string json = JsonUtility.ToJson(saveData);
        Debug.Log(json);

        using(StreamWriter writer = new StreamWriter(Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json"))
        {
            writer.Write(json);
        }
    }

    public void LoadFromFile()
    {
        string json = string.Empty;

        using(StreamReader reader = new StreamReader(Application.dataPath + Path.AltDirectorySeparatorChar + "SaveData.json"))
        {
            json = reader.ReadToEnd();
        }

        SaveData data = JsonUtility.FromJson<SaveData>(json);
        //Set PlayerData files here
    }
}