using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Health and Damage")]
    private float enemyHealth = 120f;
    private float presentHealth;
    public float giveDamage = 5f;

    [Header("Enemy Things")]
    public NavMeshAgent enemyAgent;
    public Transform LookPoint;
    public Camera ShootingRaycastArea;
    public Transform playerBody;
    public LayerMask PlayerLayer;

    [Header("Enemy guarding Var")]
    public GameObject[] walkPoints;
    int currentEnemyPosition = 0;
    public float enemySpeed;
    float walkingpointRadius = 2;

    //[Header("Sounds and UI")]

    [Header("Enemy Shooting Var")]
    public float timebtwShoot;
    bool previouslyShoot;

    //[Header("Enemy Animation and Spark Effect")]

    [Header("Enemy mood/situation")]
    public float visionRadius;
    public float shootingRadius;
    public bool playerInvisionRadius;
    public bool playerInshootingRadius;

    private void Awake()
    {
        presentHealth = enemyHealth;
        playerBody = GameObject.Find("Player").transform;
        enemyAgent= GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInvisionRadius = Physics.CheckSphere(transform.position, visionRadius, PlayerLayer);
        playerInshootingRadius = Physics.CheckSphere(transform.position, shootingRadius, PlayerLayer);

        if (!playerInshootingRadius && !playerInvisionRadius) Guard();
        if (playerInvisionRadius && !playerInshootingRadius) Pursueplayer();
        if (playerInvisionRadius && playerInshootingRadius) ShootPlayer();
    }

    private void Guard()
    {
        if (Vector3.Distance(walkPoints[currentEnemyPosition].transform.position, transform.position) < walkingpointRadius)
        {
            currentEnemyPosition = Random.Range(0, walkPoints.Length);
            if(currentEnemyPosition >= walkPoints.Length)
            {
                currentEnemyPosition= 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, walkPoints[currentEnemyPosition].transform.position, Time.deltaTime * enemySpeed);
        //changing enemy rotation
        transform.LookAt(walkPoints[currentEnemyPosition].transform.position);
    }

    private void Pursueplayer()
    {
        enemyAgent.SetDestination(playerBody.position);
        {
            //animations

            //+vision and shooting radius

            visionRadius = 80f;
            shootingRadius = 25f;
        }
    }

    private void ShootPlayer()
    {
        enemyAgent.SetDestination(transform.position);

        transform.LookAt(LookPoint);

        if(!previouslyShoot)
        {
            RaycastHit hit;
            if(Physics.Raycast(ShootingRaycastArea.transform.position, ShootingRaycastArea.transform.forward, out hit, shootingRadius))
            {
                Debug.Log("Shooting" + hit.transform.name);

                PlayerScript playerBody = hit.transform.GetComponent<PlayerScript>();

                if(playerBody != null)
                {
                    playerBody.playerHitDamage(giveDamage);
                }
            }

            previouslyShoot = true;
            Invoke(nameof(ActiveShooting), timebtwShoot);
        }
    }

    private void ActiveShooting()
    {
        previouslyShoot = false;
    }

    public void enemyHitDamage(float takeDamage)
    {
        presentHealth -= takeDamage;

        if(presentHealth <= 0)
        {
            enemyDie();
        }
    }

    private void enemyDie()
    {
        enemyAgent.SetDestination(transform.position);
        enemySpeed = 0f;
        shootingRadius = 0f;
        visionRadius = 0f;
        playerInvisionRadius = false;
        playerInshootingRadius = false;
        Object.Destroy(gameObject, 5.0f);
    }
}
