using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class demonGirl : MonoBehaviour, IDamage, IPhysics
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent Boss;
    [SerializeField] Animator animate;
    [SerializeField] Transform headPos;
    [SerializeField] LayerMask playerLayer;

    [Header("----- Enemy Stats -----")]
    [Range(0, 30)][SerializeField] int HP;
    [Range(1, 30)][SerializeField] int targetFaceSpeed;
    [Range(45, 180)][SerializeField] int viewAngle;
    [Range(45, 180)][SerializeField] int viewDistance;
    [Range(5, 50)][SerializeField] int wanderDist;
    [Range(5, 50)][SerializeField] int wanderTime;
    [SerializeField] float dodgeCooldown;
    [SerializeField] float dodgeLength;
    [SerializeField] float dodgeSpeed;
    [SerializeField] float animSpeed;
    [SerializeField] float attackAnimDelay;

    [Header("----- Weapon Stats -----")]
    [SerializeField] GameObject scythe;
    [SerializeField] Transform scytheHand;
    [SerializeField] float attackRate;
    [SerializeField] float attackRange;
    [SerializeField] int scytheDamageAmount;
    [SerializeField] int attackAngle;

    Vector3 playerDir;
    Vector3 pushBack;
    bool playerInRange;
    bool isAttacking;
    float stoppingDistOrig;
    float angleToPlayer;
    bool wanderDestination;
    Vector3 startingPos;
    Transform playerTransform;
    float origSpeed;
    bool isDodging;
    float lastDodgeTime;
    GameObject currentScythe;
    public playerController playerController;

    void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = Boss.stoppingDistance;


        playerTransform = gameManager.instance.player.transform;

        gameManager.instance.updateGameGoal(1);
    }
    void Update()
    {
        if (Boss.isActiveAndEnabled)
        {
            float agentVel = Boss.velocity.normalized.magnitude;

            animate.SetFloat("Speed", Mathf.Lerp(animate.GetFloat("Speed"), agentVel, Time.deltaTime * animSpeed));

            if (playerInRange && canViewPlayer())
            {
                animate.SetTrigger("Run");
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

                if (distanceToPlayer <= attackRange && !isAttacking)
                {
                    animate.SetTrigger("Attack");
                    StartCoroutine(meleeAttack());
                }
                else if (playerController.isFiring)
                {
                    dodge();
                }
            }
            else
            {
                animate.ResetTrigger("Run");
                StartCoroutine(wander());
            }
        }
    }
    void dodge()
    {
        Debug.Log("Dodge function called.");

        if (isDodging && Time.time > lastDodgeTime + dodgeCooldown)
        {
            StartCoroutine(dodgeMovement());
            lastDodgeTime = Time.time;
        }
    }
    IEnumerator dodgeMovement()
    {
        Debug.Log("DodgeMovement coroutine started.");

        isDodging = true;

        Vector3 dodgeDirection = transform.position - playerTransform.position;
        dodgeDirection.Normalize();
        float dodgeStart = Time.time;

        while (Time.time < dodgeStart + dodgeLength)
        {
            Boss.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
            yield return null;
        }
        isDodging = false;
    }
    IEnumerator wander()
    {
        if (Boss.remainingDistance < 0.05f && !wanderDestination)
        {
            wanderDestination = true;
            Boss.stoppingDistance = 0;
            yield return new WaitForSeconds(wanderTime);

            Vector3 randomPos = Random.insideUnitSphere * wanderDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, wanderDist, 1);
            Boss.SetDestination(hit.position);

            wanderDestination = false;
        }
    }
    bool canViewPlayer()
    {
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
#if (UNITY_EDITOR)
        Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDir);
#endif
        Debug.DrawRay(headPos.position, playerDir, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit, viewDistance, playerLayer))
        {

            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                Boss.stoppingDistance = stoppingDistOrig;
                Boss.SetDestination(gameManager.instance.player.transform.position);

                if (Boss.remainingDistance <= Boss.stoppingDistance)
                {
                    faceTarget();

                    if (!isAttacking && angleToPlayer <= attackAngle)
                    {
                        StartCoroutine(meleeAttack());
                    }
                }
                return true;
            }
        }
        Boss.stoppingDistance = 0;
        return false;
    }
    IEnumerator meleeAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            animate.SetTrigger("Attack");

            yield return new WaitForSeconds(attackAnimDelay);

            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= attackRange)
            {
                playerController player = playerTransform.GetComponent<playerController>();

                if (player != null)
                {
                    player.takeDamage(scytheDamageAmount);
                }
            }
            isAttacking = false;
        }
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        //Boss.SetDestination(gameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            animate.SetBool("Death", true);
            Boss.isStopped = true;
            gameManager.instance.updateGameGoal(-1);
        }
        else
        {

            animate.SetTrigger("Damage");
            StartCoroutine(flashDamage());
            Boss.SetDestination(gameManager.instance.player.transform.position);

        }
    }
    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }
    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * targetFaceSpeed);
    }
    public void physics(Vector3 dir)
    {
        Boss.velocity += dir / 3;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))

        {
            playerInRange = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Destroy(currentScythe);
        }
    }
}


