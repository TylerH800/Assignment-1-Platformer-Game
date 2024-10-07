//some prints or debug.logs have been left in as comments for any future testing or debugging where they might be needed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    #region variables and references
    //movement
    [Header("General movement")]
    public float moveSpeed = 7;
    public float waterSpeedMultiplier = 0.5f;
    public float jumpForce;
    private bool jumping = false;
    private bool grounded = true;
    private float groundCheckRange = 0.5f;

    public LayerMask whatIsGround;
    private Vector2 moveDir;


    //attacking
    [Header("Attacking")]
    public int attackDamage = 10;
    public float attackRange, attackVelLimit;
    private bool attacking = false;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    //scoring
    [Header("Scoring")]
    public int coinScore = 10;
    public int enemyScore = 50;
    public int crateScore = 5;

    [Header("Death")]
    public GameObject deathScreen;

    //components
    Rigidbody2D rb;
    Animator anim;
    GameManager gameManager;
    HelperScript helper;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        helper = gameObject.AddComponent<HelperScript>();

    }
    #endregion
    
    void Update()
    {
        MoveSprite();
        Jump();
        Attack();
        CheckForDeath();
        GroundCheck();

        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    helper.Test();
        //}
    }

    #region basic movement
    void MoveSprite()
    {
        if (attacking) //stops all movement if attacking
        {
            return;
        }

        //move the player left and right using the 'a' and 'd' keys, animates and flips the character
        if (Input.GetKey("a") == true)
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            anim.SetFloat("Speed", 1);           
            helper.FlipObject(true);
            moveDir = Vector2.left;
        }

        else if (Input.GetKey("d") == true)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            anim.SetFloat("Speed", 1);      
            helper.FlipObject(false);
            moveDir = Vector2.right;
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }
    }
    
    void Jump()
    {
        //checks for the player being on the ground and gets spacebar input
        if (Input.GetKeyDown("space") && grounded && !attacking)
        {
            //makes the player jump, stops them from jumping in the air and animates the jump
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            grounded = false;
            
            jumping = true;
        }

        //change the enemy animation depending on whether enemy is moving up or down
        if (rb.velocity.y >= 0.1f)
        {
            //jump anim
            anim.SetBool("IsJumping", true);

        }
        else if (rb.velocity.y < 0)
        {
            //fall anim
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", true);

        }
    }

    void GroundCheck()
    {
        //check if there is ground immediately below the enemy
        if (Physics2D.Raycast(transform.position, Vector3.down, groundCheckRange, whatIsGround))
        {
            //Debug.DrawRay(transform.position, Vector3.down);
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", false);

            grounded = true;
            jumping = false;
        }
        else
        {
            grounded = false;
        }
    }
    
    #endregion

    #region collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if you enter water, move speed is halved
        if (collision.gameObject.CompareTag("Water"))
        {
            moveSpeed *= waterSpeedMultiplier;
        }

        //if you pick up a coin, you gain score and the coin is destroyed
        if (collision.gameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            gameManager.GainScore(coinScore);
        }

        if (collision.gameObject.CompareTag("BoundsCollider"))
        {
            Die();
        }

    }

    
    private void OnTriggerExit2D(Collider2D collision)
    {
        //reverts move speed to normal once you leave water
        if (collision.gameObject.CompareTag("Water"))
        {
            moveSpeed /= waterSpeedMultiplier;
        }
    }

    #endregion

    #region combat
    void Attack()
    {
        //only allows an attack if you are on the ground and stationary
        if ( Input.GetKeyDown("e") && grounded && Mathf.Abs(rb.velocity.magnitude) < attackVelLimit)
        {
            anim.SetBool("IsAttacking", true);
            attacking = true;            
        }  
 
    }

    void AttackExecute()
    {
        //detects if an enemy is inside the attack range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D hit in hitEnemies)
        {
            //deals damage and adds score
            if (hit.transform.CompareTag("Crate"))
            {
                hit.GetComponent<BoxScript>().TakeDamage(attackDamage); 
                Debug.Log(hit.transform.name);
                gameManager.GainScore(crateScore);
            }
            else if (hit.transform.CompareTag("Enemy"))
            {
                hit.GetComponent<Enemy>().TakeDamage(attackDamage);
                Debug.Log(hit.transform.name);
                gameManager.GainScore(enemyScore);
            }            
        }        
    }

    public void AttackEnd()
    {
        anim.SetBool("IsAttacking", false);
        attacking = false;        
    }

    private void OnDrawGizmosSelected() //draws sphere visuals
    {
        if (attackPoint == null)
        {
            return; 
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);        
    }

    #endregion

    #region death
    void CheckForDeath()
    {
        //maybe temp
        if (Input.GetKey(KeyCode.Escape))
        {
            Die();
        }
    }
    public void Die()
    {
        
        Destroy(gameObject);
        deathScreen.SetActive(true); //ends the game
        
    }
    #endregion

    

}
