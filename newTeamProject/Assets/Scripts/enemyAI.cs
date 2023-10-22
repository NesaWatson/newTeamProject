using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage, IPhysics
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animate;
    [SerializeField] Transform attackPos;
    [SerializeField] Transform headPos;

    [Header("----- Enemy Stats -----")]
    [Range(0, 15)][SerializeField] int HP;
    [Range(1, 30)][SerializeField] int targetFaceSpeed;
    [Range(45, 180)][SerializeField] int viewAngle;
    [Range(5, 50)][SerializeField] int wanderDist;
    [Range(5, 50)][SerializeField] int wanderTime;
    [Range(0, 30)][SerializeField] float teleportDist;
    [Range(0, 10)][SerializeField] float teleportCooldown;
    [Range(1, 3)][SerializeField] float animSpeed;

    [Header("----- Weapon Stats -----")]
    [SerializeField] float attackRate;
    [SerializeField] int attackAngle;
    [SerializeField] GameObject shuriken;

    Vector3 playerDir;
    Vector3 playerPos;
    Vector3 pushBack;
    bool playerInRange;
    bool isAttacking;
    float stoppingDistOrig;
    float angleToPlayer;
    bool wanderDestination;
    
    Vector3 startingPos;
    enemySpawner spawner;
    Transform playerTransform;
    float origSpeed;
    bool isAlerted;
    bool canSeePlayer;
    float lastTeleportTime;
    public UiEnemyHealthBar healthBar;
    
    void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
       

        playerTransform = gameManager.instance.player.transform;
        spawner = FindObjectOfType<enemySpawner>();
        
       healthBar = GetComponentInChildren<UiEnemyHealthBar>();

        enemyManager.instance.registerEnemy(this);
        gameManager.instance.updateGameGoal(1);
    }
    void Update()
    {
        
        if (agent.isActiveAndEnabled)
        {
            if (Time.time - lastTeleportTime >= teleportCooldown)
            {
                float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);

                if (distToPlayer < teleportDist)
                {
                    teleport(playerTransform.position);
                }
            }
            faceTarget();
            float agentVel = agent.velocity.normalized.magnitude;

            animate.SetFloat("Speed", Mathf.Lerp(animate.GetFloat("Speed"), agentVel, Time.deltaTime + animSpeed));
           
            if (playerInRange && !canViewPlayer())
            {
                StartCoroutine(wander());
            }
            else if (!playerInRange)
            {
                StartCoroutine(wander());
            }
        }
    }
    void teleport(Vector3 teleportPos)
    { 
        Vector3 dirToPlayer = (teleportPos - transform.position).normalized;
        Vector3 offset = dirToPlayer * 1.0f;
        Vector3 finalTeleportPos = teleportPos + offset;

        RaycastHit hitInfo;
        NavMeshHit navMeshHit;

        if (Physics.Raycast(finalTeleportPos, Vector3.down, out hitInfo, 2.0f, LayerMask.GetMask("Ground")))
        {
            finalTeleportPos = hitInfo.point;
        }
        else
        {
           if( NavMesh.SamplePosition(finalTeleportPos, out navMeshHit, teleportDist, 1))
           {
               finalTeleportPos = navMeshHit.position;
           }
           else
           {
               finalTeleportPos = transform.position;
           }
        }
        if (NavMesh.SamplePosition(finalTeleportPos, out navMeshHit, teleportDist, 1))
        {
            agent.Warp(navMeshHit.position);
            lastTeleportTime = Time.time;
        }

    }
    public void setAlerted(Vector3 playerPos)
    {
        if (!isAlerted)
        {
            isAlerted = true;
            agent.SetDestination(playerPos);
        }
        isAlerted = false;
    }
    IEnumerator wander()
    {
        if (agent.remainingDistance < 0.05f && !wanderDestination)
        {
            wanderDestination = true;
            agent.stoppingDistance = 0;
            yield return new WaitForSeconds(wanderTime);

            agent.SetDestination(startingPos);

            Vector3 randomPos = Random.insideUnitSphere * wanderDist;
            randomPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomPos, out hit, wanderDist, 1);
            agent.SetDestination(hit.position);

            wanderDestination = false;
        }
    }
    bool canViewPlayer()
    {
        agent.stoppingDistance = stoppingDistOrig;
        playerDir = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x,0,playerDir.z), transform.forward); 
//#if(UNITY_EDITOR)
//        Debug.Log(angleToPlayer);
//        Debug.DrawRay(headPos.position, playerDir);
//#endif
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.stoppingDistance = stoppingDistOrig;
               
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();

                    if (!isAttacking && angleToPlayer <= attackAngle)
                    {
                        StartCoroutine(attack());
                    }
                } 
                

                if (!isAlerted)
                {
                    enemyManager.instance.AlertedEnemies
                        (gameManager.instance.player.transform.position);
                    isAlerted = true;
                }
               return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
    } 
    IEnumerator attack()
    {
        isAttacking = true;
        animate.SetTrigger("Attack");
        yield return new WaitForSeconds(attackRate);
        
        isAttacking= false;
    }
    public void takeDamage(int amount)
    {        
            HP -= amount;
            healthBar.SetHealth(amount);
            
            //StartCoroutine(stopMoving());

            if (HP <= 0)
            {
                agent.enabled = false;
                stopMoving();
                animate.SetBool("Death", true);
                StopAllCoroutines();
                StartCoroutine(Deadenemy());
            }
            else
            {
                Vector3 playerDirection = gameManager.instance.player.transform.position - transform.position;
                Quaternion newRotation = Quaternion.LookRotation(playerDirection);
                transform.rotation = newRotation;
                agent.SetDestination(gameManager.instance.player.transform.position);

                animate.SetTrigger("Damage");
                StartCoroutine(flashDamage());

                enemyManager.instance.AlertedEnemies(gameManager.instance.player.transform.position);
            }

    }
    IEnumerator stopMoving()
    {
        agent.speed = 0;
        yield return new WaitForSeconds(0.1f);
        agent.speed = origSpeed;
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
        agent.velocity += dir/3;
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
            agent.stoppingDistance= 0;
        }
    }
    void OnDestroy()
    {
        if(enemyManager.instance != null)
        {
            enemyManager.instance.unregisterEnemy(this);
        }
    }
    public IEnumerator Deadenemy()
    {

        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
   
}

