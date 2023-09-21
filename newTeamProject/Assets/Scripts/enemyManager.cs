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
    public void alertAllEnemies(Vector3 playerPos)
    {
        foreach (enemyAI enemy in alertedEnemies)
        {
            enemy.setAlerted(playerPos);
        }
    }
    public void AlertedEnemy(enemyAI enemy)
    {
        if (!alertedEnemies.Contains(enemy))
        {
            alertedEnemies.Add(enemy);
        }
    }
    public void UnalertedEnemies(enemyAI enemy)
    {
        alertedEnemies.Remove(enemy);
    }
}


