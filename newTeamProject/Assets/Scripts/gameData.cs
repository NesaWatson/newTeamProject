using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]

public class gameData
{
    public int playerScore;
    public bool isGameCompleted;
    public int playerHealth;
    public int playerAmmo;
    public Vector3 playerPosition;

    public gameData()
    {
        playerScore = 0;
        isGameCompleted = false;
        playerHealth = 100;
        playerAmmo = 30;
        playerPosition = Vector3.zero;
    }
}
