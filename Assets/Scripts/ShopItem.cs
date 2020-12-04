using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public GameObject buyMessage;
    private bool inBuyZone;
    public bool isHealthRestore, isHealthUpgrade, isWeapon;
    public int itemCost;
    public int healthUpgradeAmount;
    public Gun[] potentialGuns;
    private Gun theGun;
    public SpriteRenderer gunSprite;
    public Text infoText;
    // Start is called before the first frame update
    void Start()
    {
        if (isWeapon)
        {
            int selectedGun = Random.Range(0, potentialGuns.Length);
            theGun = potentialGuns[selectedGun];

            gunSprite.sprite = theGun.gunShopSprite;
            infoText.text = theGun.weaponName + "\n- " + theGun.itemCost + " gold -";
            itemCost = theGun.itemCost;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inBuyZone)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (LevelManager.instance.currentCoins >= itemCost)
                {
                    LevelManager.instance.SpendCoins(itemCost);

                    if (isHealthRestore)
                    {
                        int maxHealth = PlayerHealthController.instance.maxHealth;

                        PlayerHealthController.instance.HealPlayer(maxHealth);
                    }

                    if (isHealthUpgrade)
                    {
                        PlayerHealthController.instance.IncreaseMaxHealth(healthUpgradeAmount);
                    }

                    if (isWeapon)
                    {
                        Gun gunClone = Instantiate(theGun);
                        gunClone.transform.parent = PlayerController.instance.gunArm;
                        gunClone.transform.position = PlayerController.instance.gunArm.position;
                        gunClone.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        gunClone.transform.localScale = Vector3.one;

                        PlayerController.instance.availableGuns.Add(gunClone);
                        PlayerController.instance.currentGun = PlayerController.instance.availableGuns.Count - 1;
                        PlayerController.instance.SwitchGun();
                    }

                    gameObject.SetActive(false);
                    inBuyZone = false;

                    AudioManager.instance.PlaySFX(18);
                }
                else
                {
                    //buyMessage.GetComponent<Text>().color = new Color(255f, 0f, 0f, 1f);
                    //buyMessage.GetComponent<Text>().text = "Not enough coins!";

                    AudioManager.instance.PlaySFX(19);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            bool hasEnoughCoins = itemCost <= LevelManager.instance.currentCoins;

            buyMessage.GetComponent<Text>().text = hasEnoughCoins ? "Press 'E' to purchase" : "Not enough coins!";
            buyMessage.GetComponent<Text>().color = hasEnoughCoins ? new Color(255f, 255f, 255f, 1f) : new Color(255f, 0f, 0f, 1f);
            buyMessage.SetActive(true);

            inBuyZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            buyMessage.SetActive(false);

            inBuyZone = false;
        }
    }
}
