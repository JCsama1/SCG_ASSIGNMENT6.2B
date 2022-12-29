using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //[Header("Enemy Health and Damage")]

    [Header("Enemy Things")]
    public NavMeshAgent EnemyAgent;
    public Transform playerBody;
    public LayerMask PlayerLayer;

    [Header("Enemy guarding Var")]
    public GameObject[] walkPoints;
    int currentEnemyPosition = 0;
    public float enemySpeed;
    float walkingpointRadius = 2;

    //[Header("Sounds and UI")]

    //[Header("Enemy Shooting Var")]

    //[Header("Enemy Animation and Spark Effect")]

    [Header("Enemy mood/situation")]
    public float visionRadius;
    public float shootingRadius;
    public bool playerInvisionRadius;
    public bool playerInshootingRadius;

    private void Awake()
    {
        playerBody = GameObject.Find("Player").transform;
        EnemyAgent= GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInvisionRadius = Physics.CheckSphere(transform.position, visionRadius, PlayerLayer);
        playerInshootingRadius = Physics.CheckSphere(transform.position, shootingRadius, PlayerLayer);

        if (!playerInshootingRadius && !playerInvisionRadius) Guard();
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
}
