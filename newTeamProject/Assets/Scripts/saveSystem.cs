using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Playables;

public static class saveSystem
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
