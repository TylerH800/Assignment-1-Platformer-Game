using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrownKnife : MonoBehaviour
{
    public float speed = 20f;
    public float upForce = 1.5f;
    PlayerScript player;
    Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        
        //when instantiated, the rotation and direction is set based on the players forward direction
        
        rb.velocity = player.moveDir * speed;
        

        //adds a little initial force upwards
        rb.AddForce(Vector2.up * upForce, ForceMode2D.Impulse);
        
        if (player.moveDir.x == 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }

    }

}
