using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //variables
    public float health = 2;
    public float enemySpeed;
    public float followRad;
    public float jumpForce;
   
    bool grounded;
    bool jumping = false;
    bool chasing;
    
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    private Vector2 playerPos;

    //references
    public GameObject player;
    private Animator animator;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        GetPlayerPos();
        ChasePlayer();
        LookAtPlayer();
        
        GroundCheck();
      

    }

    void GetPlayerPos()
    {
        playerPos = player.transform.position;
    }

    void LookAtPlayer()
    {
        if (playerPos.x > transform.position.x)
        {
            sr.flipX = false;  
        }
        else
        {
            sr.flipX = true;
        }
    }

    void ChasePlayer()
    {
        if (!Physics2D.OverlapCircle(transform.position, followRad, whatIsPlayer))
        {
            chasing = false;
            //if the player is not close, do not chase
            animator.SetBool("enemyWalk", false);
            
            
        }
        else
        {
            chasing = true;
        }


        if (playerPos.x > transform.position.x && chasing)
        {
            
            rb.velocity = new Vector3(enemySpeed, rb.velocity.y);
            animator.SetBool("enemyWalk", true);

            


        }
        else if (playerPos.x < transform.position.x && chasing)
        {
            
            rb.velocity = new Vector3(-enemySpeed, rb.velocity.y);
            
            animator.SetBool("enemyWalk", true);

            


        }
    }

    

    void GroundCheck()
    {
        //check if there is ground immediately below the enemy
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.5f, whatIsGround))
        {
            //Debug.DrawRay(transform.position, Vector3.down);
            
            grounded = true;
            jumping = false;
            
        }
        else
        {
            grounded = false;
        }

        if (rb.velocity.y <= -0.5f)
        {
            animator.SetBool("enemyFall", true);
        }
        else
        {
            animator.SetBool("enemyFall", false);
        }
        

    }

    
}
