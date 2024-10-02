using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region variables and references

    //health
    public int maxHealth = 20;
    int currentHealth;

    //movement
    public float enemySpeed;
    public float followRad;
    public float jumpForce;
       
    bool chasing;

    //combat
    public float attackRad;
    public float attackCooldown;
    bool attacking = false;
    bool meleeCooldown;
    
    //player and ground detection
    public LayerMask whatIsGround;
    public LayerMask whatIsPlayer;

    private Vector2 playerPos;
    private Vector2 moveDir;

    //references
    public GameObject player;
    private Animator anim;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    HelperScript helper;

    #endregion

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        helper = gameObject.AddComponent<HelperScript>();

        currentHealth = maxHealth;
    }
    void Update()
    {
        if (player != null)
        {
            GetPlayerPos();
            LookAtPlayer();
        }             
        GroundCheck();
        StateFinder();
    }

    private void LateUpdate()
    {
        Animation();
    }
    
    void Animation()
    {
        //change the enemy animation depending on whether enemy is moving up or down
        if (rb.velocity.y >= 0.1f)
        {            
            anim.SetBool("enemyJump", true);
        }
        else if (rb.velocity.y < 0)
        {
            anim.SetBool("enemyJump", false);
            anim.SetBool("enemyFall", true );
        }

        //if the enemy is moving horizontally, play the walk animation; else, play the idle animation
        if (chasing)
        {
            anim.SetBool("enemyWalk", true);
        }
        else
        {
            anim.SetBool("enemyWalk", false);
        }
    }

    void StateFinder()
    {        
        if (Physics2D.OverlapCircle(transform.position + new Vector3(0, 1.4f, 0), attackRad, whatIsPlayer) && !meleeCooldown && !attacking)
        {
            StartAttack();
        }
        else if (Physics2D.OverlapCircle(transform.position + new Vector3(0, 1.4f, 0), followRad, whatIsPlayer) && !attacking)
        {
            ChasePlayer();
            Jumping();
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 1.4f, 0), followRad);
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 1.4f, 0), attackRad);
    }



    #region movement
    void GetPlayerPos()
    {
        //constantly finds the players position
        playerPos = player.transform.position;
    }
    void LookAtPlayer() //face the player
    {
        //flips the enemy sprite x depending on which side of the enemy the player is
        if (playerPos.x > transform.position.x)
        {
            helper.FlipObject(false);
            moveDir = Vector2.right;
        }
        else
        {
            moveDir = Vector2.left;
            helper.FlipObject(true);
        }
    }

    void ChasePlayer()   // if the player is close enough, the enemy will chase them
    {
        if (!Physics2D.OverlapCircle(transform.position, followRad, whatIsPlayer))
        {
            chasing = false;
            return;  //checks if the player is within the chase range
        }
        chasing = true;
        //directional movement
        if (playerPos.x > transform.position.x)
        {           
            rb.velocity = new Vector3(enemySpeed, rb.velocity.y);
        }
        else if (playerPos.x < transform.position.x)
        {         
            rb.velocity = new Vector3(-enemySpeed, rb.velocity.y);                        
        }
    }

    void Jumping() //if the enemy walks too close to elevated ground that it is facing, jump
    {
        //Debug.DrawRay(transform.position + new Vector3(0, 0.8f), moveDir, Color.red);
        if (Physics2D.Raycast(transform.position + new Vector3(0, 0.8f), moveDir, 1.6f, whatIsGround))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }    

    void GroundCheck()
    {
        //check if there is ground immediately below the enemy
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.5f, whatIsGround))
        {
            //Debug.DrawRay(transform.position, Vector3.down);
            anim.SetBool("enemyFall", false);
            anim.SetBool("enemyJump", false);         
        }
    }

    #endregion

    #region taking damage
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // Debug.Log(currentHealth)
        ;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //print("die");
        Destroy(gameObject);

    }

    #endregion

    #region combat mechanics

    /* plan
     * make state finder
     * 
     * Check for player being in range
     * Stop walking and wait a mo
     * Start attack animation
     * During attack animation, check for player in range (reference player script)
     * wait for a mo
     * carry on as normal
     */
    
    void StartAttack()
    {
        attacking = true;
        chasing = false;

        anim.SetBool("enemyAttack", true);
        anim.SetBool("enemyWalk", false);

        //Debug.Log("attack start");
    }

    void ExecuteAttack()
    {
        print("execute");

        Collider2D hit = Physics2D.OverlapCircle(transform.position + new Vector3(0, 1.4f, 0), attackRad, whatIsPlayer);

        hit.GetComponent<PlayerScript>().Die();


    }

    void EndAttack()
    {
        //print("resetting");
        anim.SetBool("enemyAttack", false);
        attacking = false;
        meleeCooldown = true;
        Invoke("EndMeleeCooldown", 2f);
    }

    void EndMeleeCooldown()
    {
        //print("cooldown over");
        meleeCooldown = false;
    }
    #endregion



}
