using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    private float timeBeforeFall = 1;
    
    // Start is called before the first frame update
    void Start()
    {
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
    }

    void IncreaseGrav()
    {
        rb.gravityScale = 0.6f;
    }
}
