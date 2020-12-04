using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 7.5f;
    public Rigidbody2D theRB;
    public GameObject impactEffect;
    public int damageToGive = 50;
   
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        theRB.velocity = speed * transform.right;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        
            Destroy(gameObject);
            AudioManager.instance.PlaySFX(4);
            Instantiate(impactEffect, transform.position, transform.rotation);
        

        if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyController>().DamageEnemy(damageToGive);
        }

        if(other.tag == "Boss")
        {
            BossController.instance.TakeDamage(damageToGive);

            Instantiate(BossController.instance.hitEffect, transform.position, transform.rotation);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
