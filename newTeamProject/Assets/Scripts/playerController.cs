using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController characterController;
    [SerializeField] private Transform handTransform;

    [Header("----- Player Stats -----")]
    [SerializeField] public int HP;
    [SerializeField] float characterSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] int jumpAmount;
    [Range(-35, -10)][SerializeField] float gravityPull;
    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchHeight;
    [SerializeField] float standingHeight;
    [Range(1, 10)][SerializeField] int pushBackResolve;

    [Header("----- Dodge Stats -----")]
    public float dodgeSpeed = 25f;
    public float dodgeLength = 0.2f;
    public float dodgeCooldown = 2.0f;
    private bool isDodging;
    private float lastDodgeTime = -1f;

    private bool isCrouching = false;

    [Header("----- Weapon Stats -----")]
    [SerializeField] public List<ItemStats> itemStats = new List<ItemStats>();
    [SerializeField] GameObject itemModel;
    [Range(0.2f, 2.0f)][SerializeField] float fireRate;
    [SerializeField] int gunDamage;
    [SerializeField] int shootDistance;

    [Header("----- Melee Stats -----")]
    [SerializeField] public List<meleeStats> meleeWeapons = new List<meleeStats>();
    [SerializeField] GameObject meleeWeaponModel;
    [SerializeField] float meleeAttackRate;
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeWeaponRange;

    private bool playerOnGround;
    public bool isFiring;
    private int jumps;
    private Vector3 movement;
    private Vector3 pushBack;
    private Vector3 velocity;
    int originalHP;
    int itemSelected;

    int meleeWeaponSelection;
    private bool isMeleeAttacking;
    private float lastMeleeAttack = -1f;

    private List<WeaponRuntimeData> playerGuns = new List<WeaponRuntimeData>();
    

    void Start()
    {
        originalHP = HP;
        spawnPlayer();

        if (playerGuns.Count > 0)
        {
            gameManager.instance.UpdateAmmoUI(playerGuns[itemSelected].ammoCur, playerGuns[itemSelected].config.ammoMax);
        }
    }

    void Update()
    {
        HandleMovement();
        HandleCrouch();
        itemSelect();
        SelectMeleeWeapon();


        if (itemSelected >= 0 && itemSelected < playerGuns.Count)
        {
            if (Input.GetButton("Shoot") && !isFiring && playerGuns[itemSelected].ammoCur > 0 && !isMeleeAttacking && !gameManager.instance.isPaused && playerGuns.Count > 0)
            {
                AudioSource audioSource = GetComponent <AudioSource>();
                if (audioSource != null && playerGuns[itemSelected].config.shotSound)
                {
                    audioSource.PlayOneShot(playerGuns[itemSelected].config.shotSound);
                }
                StartCoroutine(shoot());
                isFiring = true;
            }
            
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Dodge();
        }

        if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Keypad0)) && !isFiring && !gameManager.instance.isPaused && !isMeleeAttacking && meleeWeapons.Count > 0)
        {
            MeleeAttack();
        }
        
        
    }

    void HandleMovement()
    {
        if (isDodging) return;

        if (pushBack.magnitude > 0.01f)
        {


            //for push back to be individually affected on axis
            pushBack.x = Mathf.Lerp(pushBack.x, 0, Time.deltaTime * pushBackResolve);
            pushBack.y = Mathf.Lerp(pushBack.y, 0, Time.deltaTime * pushBackResolve * 3);
            pushBack.z = Mathf.Lerp(pushBack.z, 0, Time.deltaTime * pushBackResolve);

        }
        playerOnGround = characterController.isGrounded;

        if (playerOnGround && velocity.y < 0)
        {
            jumps = 0;
            velocity.y = 0f;
        }

        movement = Input.GetAxis("Horizontal") * transform.right +
            Input.GetAxis("Vertical") * transform.forward;

        float currentSpeed = isCrouching ? crouchSpeed
            * characterSpeed : characterSpeed;

        characterController.Move(movement * Time.deltaTime * characterSpeed);

        if (Input.GetButtonDown("Jump") && jumps < jumpAmount)
        {
            jumps++;
            velocity.y += jumpHeight;
        }
        velocity.y += gravityPull * Time.deltaTime;
        characterController.Move((velocity + pushBack) * Time.deltaTime);

    }

    void HandleCrouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            ToggleCrouch();
        }
    }

    void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        if (isCrouching)
        {
            characterController.height = crouchHeight;
            characterSpeed = crouchSpeed;
        }
        else characterController.height = standingHeight;
        characterSpeed = 8.0f;
    }

    IEnumerator shoot()
    {
        isFiring = true;

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)),
            out hit, shootDistance))
        {
            IDamage canDamage = hit.collider.GetComponent<IDamage>();
            Instantiate(playerGuns[itemSelected].config.hitEffect, hit.point, playerGuns[itemSelected].config.hitEffect.transform.rotation);
            if (canDamage != null)
            {
                canDamage.takeDamage(gunDamage);
            }

            playerGuns[itemSelected].ammoCur--;
            gameManager.instance.UpdateAmmoUI(playerGuns[itemSelected].ammoCur, playerGuns[itemSelected].config.ammoMax);
        }
        yield return new WaitForSeconds(playerGuns[itemSelected].config.fireRate);

        isFiring = false;
    }
    private void UpdateUi()
    {
        gameManager.instance.playerHPbar.fillAmount = (float)HP / originalHP;
    }
    public void takeDamage(int damage)
    {

        HP -= damage;
        StartCoroutine(gameManager.instance.playerDamageFlash());
        UpdateUi();
        Debug.Log("Player HP:" + HP);

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }
    public void spawnPlayer()
    {
        gameManager.instance.LoadPlayerState();
        HP = originalHP;
        UpdateUi();
        characterController.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        characterController.enabled = true;
    }

    public void itemPickup(ItemStats item)
    {
        WeaponRuntimeData newWeaponData = new WeaponRuntimeData { config = item };
        newWeaponData.RefillAmmo();
        playerGuns.Add(newWeaponData);

        gunDamage = newWeaponData.config.itemDamage;
        shootDistance = newWeaponData.config.shootDistance;
        fireRate = newWeaponData.config.fireRate;

        itemModel.GetComponent<MeshFilter>().sharedMesh = item.model.GetComponent<MeshFilter>().sharedMesh;
        itemModel.GetComponent<Renderer>().sharedMaterial = item.model.GetComponent<Renderer>().sharedMaterial;

        itemSelected = playerGuns.Count - 1;

        gameManager.instance.UpdateAmmoUI(newWeaponData.ammoCur, newWeaponData.config.ammoMax);
        gameManager.instance.ammoText.gameObject.SetActive(true);
    }

    void changeItem()
    {
        gunDamage = playerGuns[itemSelected].config.itemDamage;
        shootDistance = playerGuns[itemSelected].config.shootDistance;
        fireRate = playerGuns[itemSelected].config.fireRate;

        itemModel.GetComponent<MeshFilter>().sharedMesh = playerGuns[itemSelected].config.model.GetComponent<MeshFilter>().sharedMesh;
        itemModel.GetComponent<Renderer>().sharedMaterial = playerGuns[itemSelected].config.model.GetComponent<Renderer>().sharedMaterial;

        gameManager.instance.UpdateAmmoUI(playerGuns[itemSelected].ammoCur, playerGuns[itemSelected].config.ammoMax);
    }


    void itemSelect()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && itemSelected < playerGuns.Count - 1)
        {
            itemSelected++;
            changeItem();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && itemSelected > 0)
        {
            itemSelected--;
            changeItem();
        }
    }

    void Dodge()
    {
        if (!isDodging && Time.time > lastDodgeTime + dodgeCooldown)
        {
            StartCoroutine(DodgeMovement());
            lastDodgeTime = Time.time;
        }
    }

    IEnumerator DodgeMovement()
    {
        isDodging = true;

        Vector3 dodgeDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            dodgeDirection += transform.forward;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            dodgeDirection += -transform.right;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            dodgeDirection += -transform.forward;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            dodgeDirection += transform.right;

        dodgeDirection.Normalize();

        float dodgeStart = Time.time;

        while (Time.time < dodgeStart + dodgeLength)
        {
            characterController.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
            yield return null;
        }

        isDodging = false;
    }

    public void AmmoRefill()
    {
        foreach (WeaponRuntimeData weaponData in playerGuns)
        {
            weaponData.RefillAmmo();
        }

        if (itemSelected >= 0 && itemSelected < playerGuns.Count)
        {
            gameManager.instance.UpdateAmmoUI(playerGuns[itemSelected].ammoCur, playerGuns[itemSelected].config.ammoMax);
        }
    }

    public void PlayerCheckpointRefresh()
    {
        HP = originalHP;
        AmmoRefill();
        UpdateUi();
        
    }

    public void PickupMeleeWeapon(meleeStats meleeItem)
    {
        meleeWeapons.Add(meleeItem);

        meleeDamage = meleeItem.weaponDamage;
        meleeWeaponRange = meleeItem.weaponRange;
        meleeAttackRate = meleeItem.attackSpeed;

        meleeWeaponModel.GetComponent<MeshFilter>().sharedMesh = meleeWeapons[meleeWeaponSelection].weaponModel.GetComponent<MeshFilter>().sharedMesh;
        meleeWeaponModel.GetComponent<Renderer>().sharedMaterial = meleeWeapons[meleeWeaponSelection].weaponModel.GetComponent<Renderer>().sharedMaterial;

        meleeWeaponSelection = meleeWeapons.Count - 1;
    }

    void SelectMeleeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Q) && meleeWeaponSelection < meleeWeapons.Count - 1)
        {
            meleeWeaponSelection++;
            MeleeWeaponChange();
        }
        else if (Input.GetKeyDown(KeyCode.E) && meleeWeaponSelection > 0) 
        {
            meleeWeaponSelection--;
            MeleeWeaponChange();
        }
    }

    void MeleeWeaponChange()
    {
        meleeDamage = meleeWeapons[meleeWeaponSelection].weaponDamage;
        meleeWeaponRange = meleeWeapons[meleeWeaponSelection].weaponRange;
        meleeAttackRate = meleeWeapons[meleeWeaponSelection].attackSpeed;

        meleeWeaponModel.GetComponent<MeshFilter>().sharedMesh = meleeWeapons[meleeWeaponSelection].weaponModel.GetComponent<MeshFilter>().sharedMesh;
        meleeWeaponModel.GetComponent<Renderer>().sharedMaterial = meleeWeapons[meleeWeaponSelection].weaponModel.GetComponent <Renderer>().sharedMaterial;
    }

    void MeleeAttack()
    {
        if (meleeWeapons == null || meleeWeapons.Count == 0 || meleeWeaponSelection < 0 || meleeWeaponSelection >= meleeWeapons.Count)
        { return; }


        if (Time.time - lastMeleeAttack >= meleeAttackRate)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, meleeWeaponRange))
            {
                IDamage canDamage = hit.collider.GetComponent<IDamage>();
                if (canDamage != null) 
                {
                    canDamage.takeDamage(meleeDamage);
                }
            }

            lastMeleeAttack = Time.time;
        }
    }

    public int GetPlayerHP()
    {
        return HP;
    }

    public void SetPlayerHP(int hp)
    {
        originalHP = hp;
    }

    public int GetItemStatsCount()
    {
        return itemStats.Count;
    }

    public int GetMeleeWeaponsCount()
    {
        return meleeWeapons.Count;
    }

    public ItemStats GetItemStat(int index)
    {
        if ((index >= 0) && index < itemStats.Count)
        {
            return itemStats[index];
        }
        return null;
    }

    public meleeStats GetMeleeWeapon(int index)
    {
        if ((index >= 0) && index < meleeWeapons.Count)
        {
            return meleeWeapons[index];
        }
        return null;
    }

    public WeaponRuntimeData GetPlayerGun(int index)
    {
        if (index >= 0 && index < playerGuns.Count)
        {
            return playerGuns[index];
        }
        return null;
    }

    public ItemStats GetWeaponConfig(string weaponName)
    {
        foreach (ItemStats weapon in itemStats)
        {
            if (weapon.weaponName == weaponName)
            {
                return weapon;
            }
        }
        return null;
    }

    public int GetPlayerGunsCount() { return playerGuns.Count; }

    public void AddWeapon(string gunName, int gunAmmo)
    {
        ItemStats weaponConfig = GetWeaponConfig(gunName);
          
        if (weaponConfig != null) 
        {
            WeaponRuntimeData newWeapon = new WeaponRuntimeData { config = weaponConfig, ammoCur = gunAmmo };
            playerGuns.Add(newWeapon);
        }
    }



}

