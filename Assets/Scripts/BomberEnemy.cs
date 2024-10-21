using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class BomberEnemy : MonoBehaviour
{
    
    #region variables and references

    [Header("Standard Movement")]
    public float enemySpeed;
    public float enemyPatrolSpeed;

    public float jumpForce, jumpXForce, jumpResetTime;
    private float groundCheckRange = 1f;

    bool grounded, chasing, patrolling;
    bool canJump = true;

    private Vector2 playerPos, moveDir, jumpDir;

    private float sphereYOffest = 1.2f; //centres the spheres for player detection
    public float followRadius;

    public LayerMask whatIsGround, whatIsPlayer;
    private GameObject player;

    [Header("Combat")]
    //health
    public int damage = 100;
    public int maxHealth = 20;
    public int currentHealth;

    //attacking
    public float attackRadius, pauseTime;
    public ParticleSystem explosionParticle;

    public int scoreValue = 125;

    //dying
    bool dying = false;
    bool attacking = false;

    //references
    private Animator anim;
    private Rigidbody2D rb;
    HelperScript helper;
    GameManager gameManager;

    #endregion

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        helper = gameObject.AddComponent<HelperScript>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        player = GameObject.Find("Player");


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
        LookAtPlayer();
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
            anim.SetBool("redJump", true);
        }
        else if (rb.velocity.y < 0)
        {
            anim.SetBool("redJump", false);
            anim.SetBool("redFall", true);
        }

        //if the enemy is moving horizontally, play the walk animation; else, play the idle animation
        if (chasing || patrolling)
        {
            anim.SetBool("redWalk", true);
        }
        else
        {
            anim.SetBool("redWalk", false);
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
            Debug.DrawRay(transform.position, Vector3.down, Color.green);
            anim.SetBool("redFall", false);
            anim.SetBool("redJump", false);
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
        if (dying)
        {
            return;
        }

        //attacking
        if (Physics2D.OverlapCircle(transform.position + new Vector3(0, sphereYOffest, 0), attackRadius, whatIsPlayer) && !attacking)
        {
            StartCoroutine(BeginAttack(pauseTime));
        }
        //chasing
        else if (Physics2D.OverlapCircle(transform.position + new Vector3(0, sphereYOffest, 0), followRadius, whatIsPlayer) && !attacking)
        {
            // if the player is close enough, the enemy will chase them
            patrolling = false;
            chasing = true;

            JumpAtPlayer();            
            //print("chase");
        }
        //patrolling
        else if (grounded && !attacking)
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

    void JumpAtPlayer()
    {
        if (!canJump)
        {
            return;                        
        }

        if (playerPos.x < transform.position.x)
        {
            helper.FlipObject(true);
            jumpDir = Vector2.left;
        }
        else
        {
            helper.FlipObject(false);
            jumpDir = Vector2.right;
        }

        print("jump");
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        rb.AddForce(jumpDir * jumpXForce, ForceMode2D.Impulse);
        canJump = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (chasing)
        {
            print("canjump");
            Invoke("ResetJump", jumpResetTime);
        }
    }

    void ResetJump()
    {
        canJump = true;
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
            anim.SetBool("redHurt", true);
        }
    }

    public void EndHurtAnim()
    {
        anim.SetBool("redHurt", false);
    }

    void Die()
    {
        anim.SetBool("redDie", true);
        dying = true;
        gameManager.GainScore(scoreValue);
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    #endregion

    #region combat mechanics

    IEnumerator BeginAttack(float time)
    {
        //prevents any more movement and waits a small moment
        attacking = true;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        yield return new WaitForSeconds(time);

        //spawns the particles and detects for the player in range, before destroying
        Instantiate(explosionParticle, transform.position, Quaternion.identity);
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRadius, whatIsPlayer);
        if (hit != null && hit.transform.CompareTag("Player"))
        {
            hit.GetComponent<PlayerScript>().TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    #endregion



}
