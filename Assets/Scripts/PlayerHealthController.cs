using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;
    public int currentHealth;
    public int maxHealth;
    public float damageInvincLength = 1f;
    private float invincCount;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        maxHealth = CharacterTracker.instance.maxHealth;
        currentHealth = CharacterTracker.instance.currentHealth;

        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (invincCount > 0)
        {
            invincCount -= Time.deltaTime;

            if (invincCount <= 0)
            {
                var playerColor = PlayerController.instance.bodySR.color;

                PlayerController.instance.bodySR.color = new Color(playerColor.r, playerColor.g, playerColor.b, 1f);
            }
        }
    }

    public void DamagePlayer()
    {
        if (invincCount <= 0)
        {
            currentHealth--;

            AudioManager.instance.PlaySFX(11);


            invincCount = damageInvincLength;

            var playerColor = PlayerController.instance.bodySR.color;

            PlayerController.instance.bodySR.color = new Color(playerColor.r, playerColor.g, playerColor.b, 0.5f);

            if (currentHealth <= 0)
            {
                AudioManager.instance.PlaySFX(9);


                PlayerController.instance.gameObject.SetActive(false);

                UIController.instance.deathScreen.gameObject.SetActive(true);

                AudioManager.instance.PlayGameOver();

            }

            UIController.instance.healthSlider.value = currentHealth;
            UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
        }
    }

    public void MakeInvincible(float length)
    {
        invincCount = length;
        var playerColor = PlayerController.instance.bodySR.color;

        PlayerController.instance.bodySR.color = new Color(playerColor.r, playerColor.g, playerColor.b, 0.5f);
    }

    public void HealPlayer(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();

    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;

        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }
}
