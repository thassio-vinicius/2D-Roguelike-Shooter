using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Basic Variables")]
    public Rigidbody2D theRB;
    public float moveSpeed;
    public Animator animator;
    public int health = 150;
    public List<GameObject> deathSplatters = new List<GameObject>();
    public SpriteRenderer theBody;


    [Header("Chase Player")]
    public float rangeToChasePlayer;
    public bool shouldChasePlayer;
    private Vector3 moveDirection;


    [Header("Shoot")]
    public GameObject hitEffect;
    public bool dumbShoot;
    public bool shouldShoot;
    public GameObject bullet;
    public Transform firePoint;
    public float fireRate;
    public float shootRange;
    private float fireCounter;


    [Header("Run Away")]
    public bool shouldRunAway;
    public float runawayRange;

    [Header("Wander")]
    private float moveShiftDelay = .5f;
    public bool shouldWander;
    public float wanderLength, pauseLength;
    private float wanderCounter, pauseCounter;
    private Vector3 wanderDirection;

    [Header("Patrol")]
    public bool shouldPatrol;
    public Transform[] patrolPoints;
    private int currentPatrolPoint;

    [Header("Drop Items")]
    public bool shouldDrop;
    public GameObject[] itemsToDrop;
    public float itemDropPercent;

    // Start is called before the first frame update
    void Start()
    {
        if (shouldWander)
        {
            pauseCounter = Random.Range(pauseLength * .75f, pauseLength * 1.25f);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (theBody.isVisible && PlayerController.instance.gameObject.gameObject.activeInHierarchy)
        {

            if (shouldRunAway)
            {

                if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < runawayRange)
                {
                    //moves away from the player
                    moveDirection = transform.position - PlayerController.instance.transform.position;

                }
                else
                {
                    //moves towards the player after a delay
                    moveShiftDelay -= Time.deltaTime;

                    if (moveShiftDelay <= 0)
                    {
                        moveDirection = PlayerController.instance.transform.position - transform.position;
                        moveShiftDelay = .5f;
                    }

                }
            } else moveDirection = Vector3.zero;

            if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < rangeToChasePlayer && shouldChasePlayer)
            {
                moveDirection = PlayerController.instance.transform.position - transform.position;
            }
            else
            {
                if (shouldWander)
                {
                    if (wanderCounter > 0)
                    {
                        wanderCounter -= Time.deltaTime;

                        //move the enemy
                        moveDirection = wanderDirection;

                        if (wanderCounter <= 0)
                        {
                            pauseCounter = Random.Range(pauseLength * .75f, pauseLength * 1.25f);
                        }
                    }

                    if (pauseCounter > 0)
                    {
                        pauseCounter -= Time.deltaTime;

                        if (pauseCounter <= 0)
                        {
                            wanderCounter = Random.Range(wanderLength * .75f, wanderLength * 1.25f);

                            wanderDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
                        }
                    }
                }

                if (shouldPatrol)
                {
                    moveDirection = patrolPoints[currentPatrolPoint].position - transform.position;

                    if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < .2f)
                    {
                        currentPatrolPoint = Random.Range(0, patrolPoints.Length);

                        if (currentPatrolPoint >= patrolPoints.Length) currentPatrolPoint = 0;
                    }
                }
            }

            moveDirection.Normalize();

            theRB.velocity = moveDirection * moveSpeed;

            if (shouldShoot && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < shootRange)
            {
                fireCounter -= Time.deltaTime;


                if (fireCounter <= 0)
                {
                    fireCounter = dumbShoot ? Random.Range(fireRate * 0.5f, fireRate) : fireRate;
                    Instantiate(bullet, firePoint.position, firePoint.rotation);
                    AudioManager.instance.PlaySFX(13);

                }
            }
        }
        else
        {
            theRB.velocity = Vector2.zero;
        }

        if (moveDirection != Vector3.zero)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

    }

    public void DamageEnemy(int damage)
    {
        health -= damage;

        Instantiate(hitEffect, transform.position, transform.rotation);

        AudioManager.instance.PlaySFX(2);

        if (health <= 0)
        {
            Destroy(gameObject);

            AudioManager.instance.PlaySFX(1);

            int selectedSplatter = Random.Range(0, deathSplatters.Count);

            int rotation = Random.Range(0, 4);

            Instantiate(deathSplatters[selectedSplatter], transform.position, Quaternion.Euler(0f, 0f, rotation * 90f));

            //drop items
        if (shouldDrop)
        {
            float dropChance = Random.Range(0f, 100f);

            if (dropChance < itemDropPercent)
            {
                int randomItem = Random.Range(0, itemsToDrop.Length);

                Instantiate(itemsToDrop[randomItem], transform.position, transform.rotation);
            }
        }
        }
    }
}
