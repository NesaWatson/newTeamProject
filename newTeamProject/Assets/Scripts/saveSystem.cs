using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Playables;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]

public class gameData
{
    public int playerScore;
    public bool isGameCompleted;
    public int playerHealth;
    public int playerAmmo;
    public Vector3 playerPosition;
}
public class saveSystem : MonoBehaviour
{
    //public static void SaveGame(gameData _gameData)
    //{
    //    string json = JsonUtility.ToJson(_gameData);
    //    File.WriteAllText(Application.persistentDataPath + "/savegame.json", json);
    //}
    //public static gameData LoadGame()
    //{
    //    string path = Application.persistentDataPath + "/savegame.json";
    //    if (File.Exists(path))
    //    {
    //        string json = File.ReadAllText(path);
    //        return JsonUtility.FromJson<gameData>(json);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("No saved game data found.");
    //        return null;
    //    }
    //}
    public static void SaveGame(gameData saveData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string savePath = Application.persistentDataPath + "/save.dat";
        FileStream file = File.Create(savePath);

        formatter.Serialize(file, saveData);
        file.Close();


    }

    public static gameData LoadGame()
    {
        string savePath = Application.persistentDataPath + "/save.dat";
        if (File.Exists(savePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            gameData saveData = (gameData)formatter.Deserialize(file);
            file.Close();
            return saveData;
        }
        else
        {
            Debug.LogError("Save file not found.");
            return null;
        }
    }

}
