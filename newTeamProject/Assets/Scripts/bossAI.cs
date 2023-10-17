using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class bossAI : MonoBehaviour, IDamage, IPhysics, IBoss
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent Boss;
    [SerializeField] Animator animate;
    [SerializeField] Transform attackPos;
    [SerializeField] Transform headPos;

    [Header("----- Enemy Stats -----")]
    [Range(0, 30)][SerializeField] int HP;
    [Range(1, 30)][SerializeField] int targetFaceSpeed;
    [Range(45, 180)][SerializeField] int viewAngle;
    [Range(5, 50)][SerializeField] int wanderDist;
    [Range(5, 50)][SerializeField] int wanderTime;
    [SerializeField] float animSpeed;

    [Header("----- Weapon Stats -----")]
    [SerializeField] float attackRate;
    [SerializeField] int attackAngle;
    [SerializeField] GameObject shuriken;

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

    private bool isDefeated = false;

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

            animate.SetFloat("Speed", Mathf.Lerp(animate.GetFloat("Speed"), agentVel, Time.deltaTime + animSpeed));

            if (playerInRange && canViewPlayer())
            {
                StartCoroutine(attack());
            }
            else
            {
                StartCoroutine(wander());
            }
        }
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
        //Boss.stoppingDistance = stoppingDistOrig;
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
#if (UNITY_EDITOR)
        Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDir);
#endif
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
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
                        StartCoroutine(attack());
                    }
                }
                return true;
            }
        }
        Boss.stoppingDistance = 0;
        return false;
    }
    IEnumerator attack()
    {
        while (playerInRange)
        {
            isAttacking = true;
            animate.SetTrigger("Attack");
            yield return new WaitForSeconds(attackRate);
        }
        isAttacking = false;
    }

    public bool IsDefeated
    { get { return isDefeated; } }
    public void takeDamage(int amount)
    {
        HP -= amount;
        Boss.SetDestination(gameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            isDefeated = true;
            Boss.enabled = false;
            stopMoving();
            animate.SetBool("Death", true);
            gameManager.instance.updateGameGoal(-1);
            //Destroy(gameObject);
        }
        else
        {
            
            animate.SetTrigger("Damage");
            StartCoroutine(flashDamage());
            Boss.SetDestination(gameManager.instance.player.transform.position);

        }
    }
    IEnumerator stopMoving()
    {
        Boss.speed = 0;
        yield return new WaitForSeconds(0.1f);
        Boss.speed = origSpeed;
    }
    public void createShuriken()
    {
        Instantiate(shuriken, attackPos.position, transform.rotation);
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
        if (other.CompareTag("Player")
            )
        {
            playerInRange = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}

