using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    private float timeBeforeFall = 1;
    private float destroyTime = 4;

    public GameObject fallingPf;

    Vector2 spawnPos;


    // Start is called before the first frame update
    void Start()
    {
        spawnPos = transform.position;
        rb = GetComponent<Rigidbody2D>();       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the player lands on the platform, it will fall after one second
        if (collision.gameObject.tag == "Player")
        {  
            Invoke("Fall", timeBeforeFall);
        }
    }
    void Fall()
    {
        
        print("fall");
        rb.gravityScale = 0.3f;
        Invoke("IncreaseGrav", 1.5f);
        Invoke("Reset", 4);
    }

    void IncreaseGrav()
    {
        rb.gravityScale = 0.6f;
    }

    private void Reset()
    {
        rb.gravityScale = 0f;
        Instantiate(fallingPf, spawnPos, Quaternion.identity);
        Destroy(gameObject);
    }
}
