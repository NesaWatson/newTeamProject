using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossManager : MonoBehaviour
{
    public string bossName;

    BossUI bossHealthBar;

    bossAI Bossstatus;
    // Start is called before the first frame update
    void Start()
    {
        bossHealthBar = FindObjectOfType<BossUI>();
        Bossstatus = GetComponent<bossAI>();
    }

    // Update is called once per frame
    void Update()
    {
        bossHealthBar.SetBossName(bossName);
       // bossHealthBar.SetBossMaxHealth(Bossstatus.);
    }
}
