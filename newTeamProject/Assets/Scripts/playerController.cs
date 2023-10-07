using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController characterController;

    [Header("----- Player Stats -----")]
    [SerializeField] int HP;
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
    private bool isFiring;
    private int jumps;
    private Vector3 movement;
    private Vector3 pushBack;
    private Vector3 velocity;
    int originalHP;
    int itemSelected;

    int meleeWeaponSelection;
    private bool isMeleeAttacking;
    private MeleeController currentMeleeController;

    void Start()
    {
        originalHP = HP;
        spawnPlayer();

        if (itemStats.Count > 0)
        {
            gameManager.instance.UpdateAmmoUI(itemStats[itemSelected].ammoCur, itemStats[itemSelected].ammoMax);
        }
    }

    void Update()
    {
        HandleMovement();
        HandleCrouch();
        itemSelect();
        SelectMeleeWeapon();

        if (Input.GetButton("Shoot") && !isFiring && !gameManager.instance.isPaused && itemStats.Count > 0)
        {
            StartCoroutine(shoot());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            Dodge();
        }

        if ((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Keypad0)) && !isMeleeAttacking && !isFiring
            && !gameManager.instance.isPaused && meleeWeapons.Count > 0)
        {
            StartCoroutine(MeleeAttack());
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
            Instantiate(itemStats[itemSelected].hitEffect, hit.point, itemStats[itemSelected].hitEffect.transform.rotation);
            if (canDamage != null)
            {
                canDamage.takeDamage(gunDamage);
            }

            itemStats[itemSelected].ammoCur--;
            gameManager.instance.UpdateAmmoUI(itemStats[itemSelected].ammoCur, itemStats[itemSelected].ammoMax);
        }
        yield return new WaitForSeconds(itemStats[itemSelected].fireRate);
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
        HP = originalHP;
        UpdateUi();
        characterController.enabled = false;
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
        characterController.enabled = true;
    }

    public void itemPickup(ItemStats item)
    {
        itemStats.Add(item);

        gunDamage = item.itemDamage;
        shootDistance = item.shootDistance;
        fireRate = item.fireRate;

        itemModel.GetComponent<MeshFilter>().sharedMesh = item.model.GetComponent<MeshFilter>().sharedMesh;
        itemModel.GetComponent<Renderer>().sharedMaterial = item.model.GetComponent<Renderer>().sharedMaterial;

        itemSelected = itemStats.Count - 1;

        gameManager.instance.UpdateAmmoUI(item.ammoCur, item.ammoMax);
        gameManager.instance.ammoText.gameObject.SetActive(true);
    }

    void changeItem()
    {
        gunDamage = itemStats[itemSelected].itemDamage;
        shootDistance = itemStats[itemSelected].shootDistance;
        fireRate = itemStats[itemSelected].fireRate;

        itemModel.GetComponent<MeshFilter>().sharedMesh = itemStats[itemSelected].model.GetComponent<MeshFilter>().sharedMesh;
        itemModel.GetComponent<Renderer>().sharedMaterial = itemStats[itemSelected].model.GetComponent<Renderer>().sharedMaterial;

        gameManager.instance.UpdateAmmoUI(itemStats[itemSelected].ammoCur, itemStats[itemSelected].ammoMax);
    }


    void itemSelect()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && itemSelected < itemStats.Count - 1)
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
        foreach (ItemStats item in itemStats)
        {
            item.ammoCur = item.ammoMax;
        }

        gameManager.instance.UpdateAmmoUI(itemStats[itemSelected].ammoCur, itemStats[itemSelected].ammoMax);
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

        meleeWeaponModel.GetComponent<MeshFilter>().sharedMesh = meleeItem.weaponModel.GetComponent<MeshFilter>().sharedMesh;
        meleeWeaponModel.GetComponent<Renderer>().sharedMaterial = meleeItem.weaponModel.GetComponent<Renderer>().sharedMaterial;

        meleeWeaponSelection = meleeWeapons.Count - 1;

        currentMeleeController = meleeWeaponModel.GetComponent<MeleeController>();
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

    IEnumerator MeleeAttack()
    {
        isMeleeAttacking = true;
        StartCoroutine(SwingWeapon());
        yield return new WaitForSeconds(meleeAttackRate);
        isMeleeAttacking = false;
    }

    IEnumerator SwingWeapon()
    {
        float swingDuration = meleeAttackRate / 2;
        float timeElapsed = 0f;
        Quaternion rotationStart = meleeWeaponModel.transform.localRotation;
        Quaternion middleRotation = Quaternion.Euler(-5, -30, 0);

        currentMeleeController.ColliderEnabled();

        while (timeElapsed < swingDuration) 
        {
            timeElapsed += Time.deltaTime;
            float swing = timeElapsed / swingDuration;
            meleeWeaponModel.transform.localRotation = Quaternion.Slerp(rotationStart, middleRotation, swing);
            yield return null;
        }

        meleeWeaponModel.transform.localRotation = middleRotation;
        timeElapsed = 0f;

        while(timeElapsed < swingDuration)
        {
            timeElapsed += Time.deltaTime;
            float swing = timeElapsed / swingDuration;
            meleeWeaponModel.transform.localRotation = Quaternion.Slerp(middleRotation, rotationStart, swing);
            yield return null;
        }

        meleeWeaponModel.transform.localRotation = rotationStart;

        currentMeleeController.ColliderDisabled();
    }
}

