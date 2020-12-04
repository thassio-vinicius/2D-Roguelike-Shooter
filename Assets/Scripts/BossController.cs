using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public static BossController instance;
    public BossAction[] actions;
    private float actionCounter;
    private int currentAction;
    private float shotCounter;
    private Vector2 moveDirection;
    public Rigidbody2D theRB;
    public int currentHealth;
    public GameObject deathEffect;
    public GameObject levelExit;
    public GameObject hitEffect;
    public BossSequence[] sequences;
    public int currentSequence;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        actions = sequences[currentSequence].actions;

        actionCounter = actions[currentAction].actionLength;

        UIController.instance.bossHealth.maxValue = currentHealth;
        UIController.instance.bossHealth.value = currentHealth;

    }

    // Update is called once per frame
    void Update()
    {
        if (actionCounter > 0)
        {
            actionCounter -= Time.deltaTime;

            //handle movement
            moveDirection = Vector2.zero;

            if (actions[currentAction].shouldMove)
            {
                if (actions[currentAction].shouldChasePlayer)
                {
                    moveDirection = PlayerController.instance.transform.position - transform.position;
                    moveDirection.Normalize();
                }

                if (actions[currentAction].moveToPoints && Vector3.Distance(transform.position, actions[currentAction].pointToMoveTo.position) > .5f)
                {
                    moveDirection = actions[currentAction].pointToMoveTo.position - transform.position;
                    moveDirection.Normalize();
                }
            }

            theRB.velocity = moveDirection * actions[currentAction].moveSpeed;

            //handle shoot
            if (actions[currentAction].shouldShoot)
            {
                shotCounter -= Time.deltaTime;
                if (shotCounter <= 0)
                {
                    shotCounter = actions[currentAction].timeBetweenShots;

                    foreach (Transform shotPoint in actions[currentAction].fixedShotPoints)
                    {
                        Instantiate(actions[currentAction].itemToShoot, shotPoint.position, shotPoint.rotation);
                    }

                    if (actions[currentAction].useAllShotPoints)
                    {
                        /*
                        int counter = Random.Range(20, actions[currentAction].potentialShotPoints.Length - 1);

                        for(int i = 0; i < counter; i++)
                        {
                            int shotPointIndex = Random.Range(0, actions[currentAction].potentialShotPoints.Length);
                            Transform shotPoint = actions[currentAction].potentialShotPoints[shotPointIndex];

                            Instantiate(shotPoint, shotPoint.position, shotPoint.rotation);
                        }
                        */
                        foreach (Transform shotPoint in actions[currentAction].potentialShotPoints)
                        {
                            Instantiate(actions[currentAction].itemToShoot, shotPoint.position, shotPoint.rotation);

                        }
                    }
                }
            }
        }
        else
        {
            currentAction++;
            if (currentAction >= actions.Length)
            {
                currentAction = 0;
            }

            actionCounter = actions[currentAction].actionLength;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);

            AudioManager.instance.PlaySFX(3);

            Instantiate(deathEffect, transform.position, transform.rotation);

            levelExit.SetActive(true);

            if (Vector3.Distance(PlayerController.instance.transform.position, levelExit.transform.position) < 2f)
            {
                levelExit.transform.position += new Vector3(4f, 0f, 0f);
            }

            UIController.instance.bossHealth.gameObject.SetActive(false);

        }
        else
        {
            if (currentHealth <= sequences[currentSequence].endSequenceHealth && currentSequence < sequences.Length - 1)
            {
                currentSequence++;
                actions = sequences[currentSequence].actions;
                currentAction = 0;
                actionCounter = actions[currentAction].actionLength;
            }
        }

        UIController.instance.bossHealth.value = currentHealth;

    }
}
[System.Serializable]
public class BossSequence
{
    [Header("Sequence")]
    public BossAction[] actions;
    public int endSequenceHealth;
}

[System.Serializable]
public class BossAction
{
    [Header("Action")]
    public float actionLength;

    public bool shouldMove;
    public bool shouldChasePlayer;
    public float moveSpeed;
    public bool moveToPoints;
    public Transform pointToMoveTo;
    public bool shouldShoot;
    public GameObject itemToShoot;
    public float timeBetweenShots;
    public Transform[] potentialShotPoints;
    public Transform[] fixedShotPoints;
    public bool useAllShotPoints;

}