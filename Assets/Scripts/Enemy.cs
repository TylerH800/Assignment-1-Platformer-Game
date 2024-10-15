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
    private float groundCheckRange = 1f;
    
    bool grounded, chasing, patrolling, pause;

    private Vector2 playerPos, moveDir;

    private float sphereYOffest = 1.2f; //centres the spheres for player detection
    public float followRadius;

    public LayerMask whatIsGround, whatIsPlayer;
    private GameObject player;

    [Header("Combat")]
    //health
    public int maxHealth = 20;
    public int currentHealth;

    //attacking
    public float attackRadius;
    public float meleeCooldownLength = 2f;

    bool attacking = false;
    bool meleeCooldown;

    //dying
    bool dying = false;
        
    //references
    private Animator anim;
    private Rigidbody2D rb;
    HelperScript helper;

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        helper = gameObject.AddComponent<HelperScript>();
        player = GameObject.Find("Player");

        currentHealth = maxHealth;
    }
    void Update()
    {
       
        
            GetPlayerPos();            
        

        GroundCheck();                   
        StateFinder();
        
;    }

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
        if (player != null)
        {
            //enemy bug with not finding player pos was fixed here incase it happens here
            playerPos = player.transform.position;
            //print(player.transform.position.x);
            //print(playerPos.x);
        }
        
       
    }

    void LookAtPlayer() //face the player
    {
        if (attacking)
        {
            return;
        }
        //flips the enemy sprite x depending on which side of the enemy the player is
        if (playerPos.x > transform.position.x)
        {
            helper.FlipObject(false);
            moveDir = Vector2.right;
            //print("right");
        }
        else if (playerPos.x <= transform.position.x)
        {
            moveDir = Vector2.left;
            helper.FlipObject(true);
            //print("left");
        }
    }

    void GroundCheck()
    {
        //check if there is ground immediately below the enemy
        if (Physics2D.Raycast(transform.position, Vector3.down, groundCheckRange, whatIsGround))
        {
            Debug.DrawRay(transform.position, Vector3.down, Color.green);
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
        if (dying || attacking)
        {
            return;
        }
        LookAtPlayer();

        //attacking
        if (Physics2D.OverlapCircle(transform.position + new Vector3(0, sphereYOffest, 0), attackRadius, whatIsPlayer) && !meleeCooldown && !attacking)
        {
            
            StartAttack();
            print(attacking);
        }
        //chasing
        else if (Physics2D.OverlapCircle(transform.position + new Vector3(0, sphereYOffest, 0), followRadius, whatIsPlayer) && !attacking && !meleeCooldown)
        {
            // if the player is close enough, the enemy will chase them
            patrolling = false;
            chasing = true;

            
            ChasePlayer();
            Jumping();
            //print("chase");
        }
        //patrolling
        else if (!attacking && !meleeCooldown && grounded)
        {
            Patrolling();
            ExtendedRayCollisionCheck();
            //print("patrol");
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
        //Debug.Log("patrol");
        patrolling = true;
        rb.velocity = new Vector2(enemyPatrolSpeed, 0f);
        
        
        //changes the sprite direction based off of the movement direction
        if (rb.velocity.x > 0f)
        {
            helper.FlipObject(false);
            moveDir.x = 0.8f;
        }
        else
        {
            moveDir.x = -0.8f;
            helper.FlipObject(true);

        }
        
    }
    void ExtendedRayCollisionCheck()
    {
        //Debug.DrawRay(transform.position + new Vector3(moveDir.x, 0, 0), Vector3.down, Color.green);
        //Debug.Log(moveDir.x);
        if (!Physics2D.Raycast(transform.position + new Vector3(moveDir.x * 1.5f, 0, 0), Vector2.down, groundCheckRange, whatIsGround))
        {
            moveDir = -moveDir;
            enemyPatrolSpeed = -enemyPatrolSpeed;
        }
    }


    #endregion

    #region Chasing

    void ChasePlayer()   
    {        
        //changes direction based on the players position relative to the enemy
        if (playerPos.x > transform.position.x)
        {           
            rb.velocity = new Vector3(moveDir.x * enemySpeed, rb.velocity.y);
        }
        else if (playerPos.x < transform.position.x)
        {         
            rb.velocity = new Vector3(moveDir.x * enemySpeed, rb.velocity.y);                        
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BoundsCollider"))
        {
            Die();
        }
    }



    #endregion

    #region taking damage

    public void TakeDamage(int damage)
    {
        //takes damage based on a value passed in
        currentHealth -= damage;
        Debug.Log(currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            anim.SetBool("enemyHurt", true);
        }
    }
    
    public void EndHurtAnim()
    {
        anim.SetBool("enemyHurt", false);
    }

    void Die()
    {
        anim.SetBool("enemyDie", true);
        dying = true;
    }

    public void Despawn()
    {
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
        if (hit != null)
        {            
            hit.GetComponent<PlayerScript>().StartDeath();
        }
    }


    void EndAttack()
    {
        //lets the enemy move again and starts an attack cooldown timer, preventing the enemy from attacking straight away
        print("resetting");
        anim.SetBool("enemyAttack", false);
        attacking = false;
        pause = true;
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
