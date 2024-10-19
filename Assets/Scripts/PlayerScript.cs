//some prints or debug.logs have been left in as comments for any future testing or debugging where they might be needed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    #region variables and references
    //movement
    [Header("General movement")]
    public float moveSpeed = 7;
    public float waterSpeedMultiplier = 0.5f;
    public float jumpForce;
    private bool grounded = true;
    private float groundCheckRange = 0.5f;

    private Vector2 moveInputValue;

    public LayerMask whatIsGround;
    public Vector2 moveDir;


    //attacking
    [Header("Attacking")]
    public int attackDamage = 10;
    public float attackRange, attackVelLimit;
    private bool attacking = false;
    private bool canThrow = true;
    public float throwCooldown = 0.5f;
    public Transform attackPoint;
    public Transform shootPoint;
    public LayerMask enemyLayers;
    public GameObject knifePrefab;

    [Header("Health")]
    public int maxHealth;
    public int currentHealth;

    //scoring
    [Header("Scoring")]
    public int coinScore = 10;
    public int crateScore = 5;

    [Header("Death")]
    public GameObject deathScreen;
    bool dying = false;

    [Header("Audio")]
    public AudioSource source;
    public AudioClip knife;
    public AudioClip knifeThrow;
    public AudioClip jump;


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

        currentHealth = maxHealth;

    }
    #endregion
    
    void Update()
    {
        MoveSprite();
        Jump();
        Attack();
        CheckForEsc();
        GroundCheck();

        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    helper.Test();
        //}
    }

    #region basic movement

    void OnMove(InputValue value)
    {
        //gets input from the input manager
        moveInputValue = value.Get<Vector2>();
        //Debug.Log(moveInputValue);
    }

    void MoveSprite()
    {
        if (attacking || dying || gameManager.won) //stops all movement if attacking
        {
            return;
        }

        float xInput = moveInputValue.x * moveSpeed * Time.deltaTime;
        //Debug.Log(xInput);
        
        //move the player left and right using the 'a' and 'd' keys, animates and flips the character
        if (xInput < 0 || Input.GetKey("a"))
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            anim.SetFloat("Speed", 1);           
            helper.FlipObject(true);
            moveDir = Vector2.left;
        }
        

        else if (xInput > 0 || Input.GetKey("d"))
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

        if (dying || !grounded || attacking || gameManager.won)
        {
            return; //jump can only run if certain conditions are met
        }
        
        if (Input.GetButtonDown("Jump"))
        {
            //makes the player jump, stops them from jumping in the air and animates the jump
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            grounded = false;

            //plays the jump sound
            source.clip = jump;
            source.Play();
            
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
        //if you pick up a coin, you gain score and the coin is destroyed
        if (collision.gameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            gameManager.GainScore(coinScore);
        }

        //kills you if you fall too far
        if (collision.gameObject.CompareTag("BoundsCollider"))
        {
            Die();
        }

    }

    #endregion

    #region combat
    void Attack()
    {
        //things that prohibt you from attacking
        if (gameManager.won || dying || !grounded || Mathf.Abs(rb.velocity.magnitude) > attackVelLimit)
        {
            return;
        }

        //melee attack
        if (Input.GetButtonDown("Fire1"))
        {
            anim.SetBool("IsAttacking", true);
            attacking = true;       
            
            //plays the knife attack sound
            source.clip = knife;
            source.Play();
        }  

        //ranged attack
        if (Input.GetButtonDown("Fire2") && canThrow)
        {
            Instantiate(knifePrefab, shootPoint.position, Quaternion.Euler(moveDir));
            //stops you from throwing again for a set time
            canThrow = false;
            Invoke("CanThrow", throwCooldown);

            //plays the knife throw sound
            source.clip = knifeThrow;
            source.Play();
        }
 
    }

    void AttackExecute()
    {
        //Debug.Log("attack");
        //detects if an enemy is inside the attack range
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRange, enemyLayers);

        if (hit == null)
        {
            return;
        }
       
        //deals damage and adds score
        if (hit.transform.CompareTag("Crate"))
        {
            hit.GetComponent<BoxScript>().TakeDamage(attackDamage); 
            //Debug.Log(hit.transform.name);
            gameManager.GainScore(crateScore);
        }
        else if (hit.transform.CompareTag("Enemy"))
        {
            hit.GetComponent<Enemy>().TakeDamage(attackDamage);
            //Debug.Log(hit.transform.name);            
        }
                      
    }

    public void AttackEnd()
    {
        anim.SetBool("IsAttacking", false);
        attacking = false;        
    }

    private void OnDrawGizmosSelected() //draws sphere visuals
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }       
    }

    void CanThrow()
    {
        //lets you throw a knife again
        canThrow = true;
    }
        
    #endregion

    #region death and damage
    void CheckForEsc()
    {
        //if you press escape the game ends
        if (Input.GetKey(KeyCode.Escape))
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        //takes damage based on a value passed in
        currentHealth -= damage;
        //Debug.Log(currentHealth);

        if (currentHealth <= 0)
        {            
            anim.SetBool("IsDying", true);
            dying = true;
        }
        else
        {
            anim.SetBool("IsHurt", true);
        }
    }

    void EndHurtAnim()
    {
        anim.SetBool("IsHurt", false);
    }
 
    public void Die()
    {
        //death sound is played in game manager because this game object is destroyed
        Destroy(gameObject);
        deathScreen.SetActive(true); //ends the game
        gameManager.DisplayDeathScreen();
        
    }
    #endregion
}
