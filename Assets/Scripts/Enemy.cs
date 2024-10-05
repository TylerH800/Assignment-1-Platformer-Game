//some prints or debug.logs have been left in as comments for any future testing or debugging where they might be needed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region variables and references

    [Header("Standard Movement")]
    public float enemySpeed;
    public float enemyPatrolSpeed;


    public float jumpForce;
    public float jumpDetectRange = 1.6f;
    private float rayYOffset = 0.8f;
    private float groundCheckRange = 0.5f;
    
    bool grounded, chasing, patrolling;
    bool canTurn = true;

    private Vector2 playerPos, moveDir;

    private float sphereYOffest = 1.4f; //centres the spheres for player detection
    public float followRadius;

    public LayerMask whatIsGround, whatIsPlayer;
    public GameObject player;

    [Header("Combat")]
    //health
    public int maxHealth = 20;
    int currentHealth;

    //attacking
    public float attackRadius;
    public float meleeCooldownLength = 2f;

    bool attacking = false;
    bool meleeCooldown;

    public ParticleSystem bloodSplatter;


    //references
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
        if (chasing || patrolling)
        {
            anim.SetBool("enemyWalk", true);
        }
        else
        {
            anim.SetBool("enemyWalk", false);
        }
    }

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

    void GroundCheck()
    {
        //check if there is ground immediately below the enemy
        if (Physics2D.Raycast(transform.position, Vector3.down, groundCheckRange, whatIsGround))
        {
            //Debug.DrawRay(transform.position, Vector3.down);
            anim.SetBool("enemyFall", false);
            anim.SetBool("enemyJump", false);
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }


    #region State Finding

    void StateFinder()
    {        
        //attacking
        if (Physics2D.OverlapCircle(transform.position + new Vector3(0, sphereYOffest, 0), attackRadius, whatIsPlayer) && !meleeCooldown && !attacking)
        {
            LookAtPlayer();
            StartAttack();
        }
        //chasing
        else if (Physics2D.OverlapCircle(transform.position + new Vector3(0, sphereYOffest, 0), followRadius, whatIsPlayer) && !attacking)
        {
            // if the player is close enough, the enemy will chase them
            patrolling = false;
            chasing = true;

            LookAtPlayer();
            ChasePlayer();
            Jumping();
        }
        //patrolling
        else if (!attacking)
        {
            Patrolling();
        }        
    }

    private void OnDrawGizmosSelected()
    {
        //draws the spheres for attack range and chase range
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, sphereYOffest, 0), followRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, sphereYOffest, 0), attackRadius);
    }

    #endregion

    #region Patrolling

    void Patrolling()
    {
        patrolling = true;
        rb.velocity = new Vector2(enemyPatrolSpeed, 0f);
        
        //changes the sprite direction based off of the movement direction
        if (rb.velocity.x > 0f)
        {
            helper.FlipObject(false);
        }
        else
        {
            helper.FlipObject(true);

        }
        
        //makes the enemy move in the opposite direction if it reaches the end of a platform
        if (!grounded && canTurn)
        {
            enemyPatrolSpeed *= -1f;
            canTurn = false;
            Invoke("TurnAroundReset", 1f); //prevents the enemy from getting stuck in a loop of turning round every frame
        }
    }

    void TurnAroundReset()
    {
        canTurn = true;
    }

    #endregion

    #region Chasing

    void ChasePlayer()   
    {        
        //changes direction based on the players position relative to the enemy
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
        if (Physics2D.Raycast(transform.position + new Vector3(0, rayYOffset), moveDir, jumpDetectRange, whatIsGround))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }    

    

    #endregion

    #region taking damage

    public void TakeDamage(int damage)
    {
        //takes damage based on a value passed in
        currentHealth -= damage;
        // Debug.Log(currentHealth)
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //print("die");
        Instantiate(bloodSplatter, transform.position + new Vector3(0, 1.5f), Quaternion.identity);
        Destroy(gameObject);

    }

    #endregion

    #region combat mechanics
    
    void StartAttack()
    {
        //starts animation and prevents movement
        attacking = true;
        patrolling = false;
        chasing = false;

        anim.SetBool("enemyAttack", true);
        anim.SetBool("enemyWalk", false);

        //Debug.Log("attack start");
    }

    void ExecuteAttack()
    {
        //checks for a player in the hit range, and kills it if present
        //print("execute");
        Collider2D hit = Physics2D.OverlapCircle(transform.position + new Vector3(0, sphereYOffest, 0), attackRadius, whatIsPlayer);
        Instantiate(bloodSplatter, hit.transform.position + new Vector3(0, 1), Quaternion.identity);
        hit.GetComponent<PlayerScript>().Die();
    }

    void EndAttack()
    {
        //lets the enemy move again and starts an attack cooldown timer, preventing the enemy from attacking straight away
        //print("resetting");
        anim.SetBool("enemyAttack", false);
        attacking = false;
        meleeCooldown = true;
        Invoke("EndMeleeCooldown", meleeCooldownLength);
    }

    void EndMeleeCooldown()
    {
        //lets the enemy attack again
        //print("cooldown over");
        meleeCooldown = false;
    }
    
    #endregion



}
