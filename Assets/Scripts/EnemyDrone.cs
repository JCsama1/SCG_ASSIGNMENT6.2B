using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDrone : MonoBehaviour
{
    [Header("Enemy Drone Health and Damage")]
    private float enemyHealth = 120f;
    private float presentHealth;
    public float giveDamage = 3f;

    [Header("Enemy Drone Things")]
    public NavMeshAgent enemyAgent;
    public Transform LookPoint;
    public Camera ShootingRaycastArea;
    public Transform playerBody;
    public LayerMask PlayerLayer;

    [Header("Enemy Drone guarding Var")]
    public GameObject[] walkPoints;
    int currentEnemyPosition = 0;
    public float enemySpeed;
    float walkingpointRadius = 2;

    //[Header("Sounds and UI")]

    [Header("Enemy Drone Shooting Var")]
    public float timebtwShoot;
    bool previouslyShoot;

    [Header("Enemy Drone Animation and Spark Effect")]
    public Animator anim;
    public ParticleSystem muzzleSpark;
    public ParticleSystem muzzleFlame;
    public ParticleSystem DestroyEffect;

    [Header("Enemy Drone mood/situation")]
    public float visionRadius;
    public float shootingRadius;
    public bool playerInvisionRadius;
    public bool playerInshootingRadius;

    private void Awake()
    {
        presentHealth = enemyHealth;
        playerBody = GameObject.Find("Player").transform;
        enemyAgent = GetComponent<NavMeshAgent>();
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
            if (currentEnemyPosition >= walkPoints.Length)
            {
                currentEnemyPosition = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, walkPoints[currentEnemyPosition].transform.position, Time.deltaTime * enemySpeed);
        //changing enemy rotation
        transform.LookAt(walkPoints[currentEnemyPosition].transform.position);
    }

    private void Pursueplayer()
    {
        if (enemyAgent.SetDestination(playerBody.position))
        {
            //animations
            anim.SetBool("Walk", false);
            anim.SetBool("AimRun", true);
            anim.SetBool("Shoot", false);
            anim.SetBool("Die", false);


            //+vision and shooting radius

            visionRadius = 30f;
            shootingRadius = 16f;
        }
        else
        {
           anim.SetBool("Walk", false);
           anim.SetBool("AimRun", false);
           anim.SetBool("Shoot", false);
           anim.SetBool("Die", true);
        }
    }

    private void ShootPlayer()
    {
        enemyAgent.SetDestination(transform.position);

        transform.LookAt(LookPoint);

        if (!previouslyShoot)
        {

            muzzleSpark.Play();
            muzzleFlame.Play();

            RaycastHit hit;
            if (Physics.Raycast(ShootingRaycastArea.transform.position, ShootingRaycastArea.transform.forward, out hit, shootingRadius))
            {
                Debug.Log("Shooting" + hit.transform.name);

                PlayerScript playerBody = hit.transform.GetComponent<PlayerScript>();

                if (playerBody != null)
                {
                    playerBody.playerHitDamage(giveDamage);
                }

                anim.SetBool("Shoot", true);
                anim.SetBool("Walk", false);
                anim.SetBool("AimRun", false);
                anim.SetBool("Die", false);

            }

            previouslyShoot = true;
            Invoke(nameof(ActiveShooting), timebtwShoot);
        }
    }

    private void ActiveShooting()
    {
        previouslyShoot = false;
    }

    public void enemyDroneHitDamage(float takeDamage)
    {
        presentHealth -= takeDamage;

        if (presentHealth <= 0)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Shoot", false);
            anim.SetBool("AimRun", false);
            anim.SetBool("Die", true);

            enemyDie();
        }
    }

    private void enemyDie()
    {
        DestroyEffect.Play();
        enemyAgent.SetDestination(transform.position);
        enemySpeed = 0f;
        shootingRadius = 0f;
        visionRadius = 0f;
        playerInvisionRadius = false;
        playerInshootingRadius = false;
        Object.Destroy(gameObject, 5.0f);
    }
}

