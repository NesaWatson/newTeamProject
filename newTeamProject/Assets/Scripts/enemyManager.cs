using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyManager : MonoBehaviour
{
    public static enemyManager instance;
    private List<enemyAI> alertedEnemies = new List<enemyAI>();

    private void Awake()
    {
        instance = this;
    }
    public void AlertedEnemies(Vector3 playerPos)
    {
        foreach (var enemy in alertedEnemies)
        {
            enemy.setAlerted(playerPos);
        }
    }
    public void registerEnemy(enemyAI enemy)
    {
        alertedEnemies.Add(enemy);
    }
    public void unregisterEnemy(enemyAI enemy)
    {
        alertedEnemies.Remove(enemy);
    }
}


