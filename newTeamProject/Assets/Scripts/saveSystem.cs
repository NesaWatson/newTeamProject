using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Playables;

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
    public static void SaveGame(gameData _gameData)
    {
        string json = JsonUtility.ToJson(_gameData);
        File.WriteAllText(Application.persistentDataPath + "/savegame.json", json);
    }
    public static gameData LoadGame()
    {
        string path = Application.persistentDataPath + "/savegame.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<gameData>(json);
        }
        else
        {
            Debug.LogWarning("No saved game data found.");
            return null;
        }
    }
}
