using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    public GameObject player;
    public playerController playerScript;

    public Image playerHPbar;
    public TMP_Text ammoText;
    

    [SerializeField] GameObject activeMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject playerFlashDamage;
    [SerializeField] GameObject checkPointMenu;
    [SerializeField] TMP_Text enemiesRemainingText;
    
    public GameObject playerSpawnPos;

    [SerializeField] int enemiesRemaining;

    public bool isPaused;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        playerSpawnPos = GameObject.FindGameObjectWithTag("Player Spawn Pos");
        ammoText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu == null)
        {
            statePause();
            activeMenu = pauseMenu;
            activeMenu.SetActive(isPaused);
        }
    }
    public void statePause()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        isPaused = !isPaused;
    }
    public void stateUnpause()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = !isPaused;
        activeMenu.SetActive(isPaused);
        activeMenu = null;
    }
    IEnumerator youWin()
    {
        yield return new WaitForSeconds(3);
        statePause();
        activeMenu = winMenu;
        activeMenu.SetActive(isPaused);
    }
    public void updateGameGoal(int amount)
    {
        enemiesRemaining = amount;

        enemiesRemainingText.text = enemiesRemaining.ToString("0");

        if (enemiesRemaining <= 0)
        {

            StartCoroutine(youWin());
        }
    }
    public void youLose()
    {
        statePause();
        activeMenu = loseMenu;
        activeMenu.SetActive(isPaused);
    }

    public IEnumerator playerDamageFlash()
    {
        playerFlashDamage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerFlashDamage.SetActive(false);
    }
   public IEnumerator checkPointSpot()
   {
        checkPointMenu.SetActive(true);
        yield return new WaitForSeconds(2);
        checkPointMenu.SetActive(false);
   }

   public void UpdateAmmoUI(int currentAmmo, int maxAmmo)
   {
        ammoText.text = $"{currentAmmo}/{maxAmmo}";
   }

    public void SavePlayerState()
    {
        //save player's current state upon completing level
        int playerHP = playerScript.GetPlayerHP();
        PlayerPrefs.SetInt("Player_HP", playerHP);


        int itemCount = playerScript.GetItemStatsCount();
        PlayerPrefs.SetInt("ItemStats_Count", itemCount);
        for (int i = 0; i < itemCount; i++) 
        {
            ItemStats item = playerScript.GetItemStat(i);
            if (item != null) 
            {
                PlayerPrefs.SetString($"Gun_{i}", item.weaponName);
            }
        }
        int meleeCount = playerScript.GetMeleeWeaponsCount();
        PlayerPrefs.SetInt("MeleeWeapons_Count", meleeCount);
        for (int i = 0;i < meleeCount; i++)
        {
            meleeStats meleeWeapon = playerScript.GetMeleeWeapon(i);
            if (meleeWeapon != null) 
            {
                PlayerPrefs.SetString($"Melee_{i}", meleeWeapon.weaponName);
            }
        }
    }
  
   
}
