using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [Header("~~~~~ Components ~~~~~")]
    [SerializeField] CharacterController characterController;

    [Header("~~~~~ Player Stats ~~~~~")]
    [SerializeField] int HP;
    [SerializeField] float characterSpeed;
    [SerializeField] float jumpHeight;
    [SerializeField] int jumpAmount;
    [SerializeField] float gravityPull;
    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchHeight;
    [SerializeField] float standingHeight;

    private bool isCrouching = false;

    [Header("~~~~~ Weapon Stats ~~~~~")]
    [SerializeField] List<ItemStats> itemStats = new List<ItemStats>();
    [SerializeField] GameObject itemModel;
    [SerializeField] float fireRate;
    [SerializeField] int gunDamage;
    [SerializeField] int shootDistance;

    private bool playerOnGround;
    private bool isFiring;
    private int jumps;
    private Vector3 movement;
    private Vector3 pushBack;
    private Vector3 velocity;
    int originalHP;
    int itemSelected;

    void Start()
    {
        originalHP = HP;
        spawnPlayer();
    }
    
    void Update()
    {
        HandleMovement();
        HandleCrouch();
        itemSelect();

        if (Input.GetButton("Shoot") && !isFiring) 
        {
            StartCoroutine(shoot());
        }
    }

    void HandleMovement()
    {
        playerOnGround = characterController.isGrounded;

        if (playerOnGround && velocity.y < 0) 
        {
            jumps =0;
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
        characterController.Move((velocity + pushBack)  * Time.deltaTime);

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

            if (canDamage != null) 
            {
                canDamage.takeDamage(gunDamage);
            }
        }
        yield return new WaitForSeconds(fireRate);
        isFiring = false;
    }
  
    public void takeDamage(int damage) 
    {

        HP -= damage;
        StartCoroutine(gameManager.instance.playerDamageFlash());
        Debug.Log("Player HP:" + HP);

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }
    public void spawnPlayer()
    {
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
    }

    void changeItem()
    {
        gunDamage = itemStats[itemSelected].itemDamage;
        shootDistance = itemStats[itemSelected].shootDistance;
        fireRate -= itemStats[itemSelected].fireRate;

        itemModel.GetComponent<MeshFilter>().sharedMesh = itemStats[itemSelected].model.GetComponent<MeshFilter>().sharedMesh;
        itemModel.GetComponent<Renderer>().sharedMaterial = itemStats[itemSelected ].model.GetComponent<Renderer>().sharedMaterial;

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
}

