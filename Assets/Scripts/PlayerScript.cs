using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    #region variables and references
    //movement
    public float moveSpeed = 7;
    public float jumpForce;
    public bool grounded = true;
    private bool jumping = false;

    public LayerMask whatIsGround;
    private Vector2 moveDir;

    //attacking
    private bool attacking = false;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayers;


    //components
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    HelperScript helper;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

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

        if (Input.GetKeyDown(KeyCode.H))
        {
            helper.Test();
        }
       
    }

    #region basic movement
    void MoveSprite()
    {
        if (attacking)
        {
            return;
        }
        
        anim.SetFloat("Speed", 0);

        //move the player left and right using the 'a' and 'd' keys, and animate the character
        if (Input.GetKey("a") == true)
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
            anim.SetFloat("Speed", 1);
            sr.flipX = true;
            moveDir = Vector2.left;
        }

        if (Input.GetKey("d") == true)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
            anim.SetFloat("Speed", 1);
            sr.flipX = false;
            moveDir = Vector2.right;

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
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.5f, whatIsGround))
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

    #region water interaction
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            moveSpeed /= 2;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            moveSpeed *= 2;
        }
    }

    #endregion

    #region combat
    void Attack()
    {
        if ( Input.GetKeyDown("e") && grounded && rb.velocity.magnitude < 0.5f)
        {
            anim.SetBool("IsAttacking", true);
            attacking = true;            
        }  
 
    }

    void AttackExecute()
    {
        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D hit in hitEnemies)
        {
            if (hit.transform.CompareTag("Crate"))
            {
                hit.GetComponent<BoxScript>().TakeDamage(attackDamage); 
                Debug.Log(hit.transform.name);
            }
            else if (hit.transform.CompareTag("Enemy"))
            {
                hit.GetComponent<Enemy>().TakeDamage(attackDamage);
                Debug.Log(hit.transform.name);
            }
            
        }
        
    }

    public void AttackEnd()
    {
        anim.SetBool("IsAttacking", false);
        attacking = false;        
    }

    private void OnDrawGizmosSelected()
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
        if (transform.position.y < -10 || Input.GetKey(KeyCode.Escape))
        {
            Die();
        }

    }
    public void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

}
