using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public GameObject bulletToFire;
    public Transform firePoint;
    public float timeBetweenShots;
    private float shotCounter;
    public string weaponName;
    public Sprite gunUI;
    public int itemCost;
    public Sprite gunShopSprite;

    [Header("Shotgun")]
    public bool isShotgun;
    public Transform[] shotPoints;
    public float bulletLifespan;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.instance.canMove && !LevelManager.instance.isPaused)
        {
            if (shotCounter > 0)
            {
                shotCounter -= Time.deltaTime;
            }
            else
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                {
                    AudioManager.instance.PlaySFX(4);

                    if (isShotgun)
                    {
                        List<GameObject> bullets = new List<GameObject>();
                        foreach (Transform t in shotPoints)
                        {
                            bullets.Add(Instantiate(bulletToFire, t.position, t.rotation));

                            foreach(GameObject bullet in bullets)
                                {
                                    Destroy(bullet, bulletLifespan);
                                }

                        }
                    }
                    else
                    {
                        Instantiate(bulletToFire, firePoint.position, firePoint.rotation);

                    }

                    shotCounter = timeBetweenShots;
                }
            }
        }
    }
}
