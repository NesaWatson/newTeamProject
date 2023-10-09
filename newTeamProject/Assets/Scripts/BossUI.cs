using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossUI : MonoBehaviour
{
    public Text BossName;
    Slider slider;

    private void Awake()
    {
        slider =GetComponentInChildren<Slider>();
        BossName= slider.GetComponentInChildren<Text>();
    }
    private void Start()
    {
        slider.gameObject.SetActive(false);
    }

    public void SetBossName(string name)
    {
        BossName.text = name;
    }
    public void SetUIHealthBarToActive()
    {
        slider.gameObject.SetActive(true);
    }
    public void SethealthBarInactive()
    {
        slider.gameObject.SetActive(false);
    }
    public void SetBossMaxHealth(int maxHealth)
    {
        slider.maxValue= maxHealth;
        slider.value= maxHealth;
    }
    public void SetBossCurrentHealth(int currentHealth)
    {
        slider.value= currentHealth;
    }
}
