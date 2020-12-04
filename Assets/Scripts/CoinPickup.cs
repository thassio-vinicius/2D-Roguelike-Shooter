using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;
    public float waitToBeCollected = .5f;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (waitToBeCollected > 0)
        {
            waitToBeCollected -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && waitToBeCollected <= 0)
        {
            LevelManager.instance.GetCoins(coinValue);

            AudioManager.instance.PlaySFX(5);
            Destroy(gameObject);
        }

    }
}
