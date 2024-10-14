using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrownKnife : MonoBehaviour
{
    public float speed = 20f;
    public float upForce = 1.5f;
    public int damage = 10;
    public float destroyTime = 2.5f;
    
    private PlayerScript player;
    private Rigidbody2D rb;

    public ParticleSystem hitParticle;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        
        //when instantiated, the rotation and direction is set based on the players forward direction
        rb.velocity = player.moveDir * speed;

        if (player.moveDir.x == 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }

        //adds a little initial force upwards
        rb.AddForce(Vector2.up * upForce, ForceMode2D.Impulse);

        //Destroys the game object after a set time
        Destroy(gameObject, destroyTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if an enemy is hit, it's script is found and damage is taken, before the knife is destroyed
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponent<Enemy>() != null)
            {
                collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
                Instantiate(hitParticle, transform.position, Quaternion.identity);
                Destroy(gameObject);
            } 

            else if (collision.gameObject.GetComponent<BomberEnemy>() != null)
            {
                collision.gameObject.GetComponent<BomberEnemy>().TakeDamage(damage);
                Instantiate(hitParticle, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            
        }

        //if the ground or another solid object is hit, the knife is destroyed
        else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Crate"))
        {
            Instantiate(hitParticle, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

}
