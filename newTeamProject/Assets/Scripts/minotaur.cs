using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class minotaur : MonoBehaviour, IDamage, IPhysics
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent FinalBoss;
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
    [SerializeField] GameObject axe;
    [SerializeField] Transform axeHand;
    [SerializeField] float attackRate;
    [SerializeField] float attackRange;
    [SerializeField] int axeDamageAmount;
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
    GameObject currentAxe;
    public playerController playerController;
    //private bool isDefeated = false;

    void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = FinalBoss.stoppingDistance;


        playerTransform = gameManager.instance.player.transform;

        gameManager.instance.updateGameGoal(1);
    }
    void Update()
    {
        //if (!isDefeated)
        
            if (FinalBoss.isActiveAndEnabled)
            {
                float agentVel = FinalBoss.velocity.normalized.magnitude;

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
            FinalBoss.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
            yield return null;
        }
        isDodging = false;
    }
    IEnumerator wander()
    {
        if (FinalBoss.remainingDistance < 0.05f && !wanderDestination)
        {
            wanderDestination = true;
            FinalBoss.stoppingDistance = 0;
            yield return new WaitForSeconds(wanderTime);

            Vector3 randomPos = Random.insideUnitSphere * wanderDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, wanderDist, 1);
            FinalBoss.SetDestination(hit.position);

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
                FinalBoss.stoppingDistance = stoppingDistOrig;
                FinalBoss.SetDestination(gameManager.instance.player.transform.position);

                if (FinalBoss.remainingDistance <= FinalBoss.stoppingDistance)
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
        FinalBoss.stoppingDistance = 0;
        return false;
    }
    IEnumerator meleeAttack()
    {
        //if (isDefeated) yield break;
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
                    player.takeDamage(axeDamageAmount);
                    Debug.Log("Player took damage");

                }
            }
            isAttacking = false;
        }
    }

    //public bool IsDefeated
    //{
    //    get { return isDefeated; }
    //}

    public void takeDamage(int amount)
    {
        HP -= amount;
        //FinalBoss.SetDestination(gameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            //isDefeated = true;
            animate.SetBool("Death", true);
            FinalBoss.isStopped = true;
            gameManager.instance.updateGameGoal(-1);
        }
        else
        {

            animate.SetTrigger("Damage");
            StartCoroutine(flashDamage());
            FinalBoss.SetDestination(gameManager.instance.player.transform.position);

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
        FinalBoss.velocity += dir / 3;
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
            Destroy(currentAxe);
        }
    }
}





