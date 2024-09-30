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
    public  float attackRange;


    //components
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;
    public GameObject attackBox;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

       

    }
    #endregion
    
    void Update()
    {
        MoveSprite();
        Jump();
        Attack();
        CheckForDeath();
        GroundCheck();
        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), moveDir, Color.red);
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
            anim.SetBool("IsJumping", true);
            jumping = true;
        }

        if (!grounded && !jumping)
        { //if the player falls but doesnt jump, play the falling animation
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

    #region attacking
    void Attack()
    {
        if ( Input.GetKeyDown("e") && grounded && rb.velocity == Vector2.zero)
        {
            anim.SetBool("IsAttacking", true);
            attacking = true;
            //attackBox.SetActive(true);
        }  
        
    }

    void AttackExecute()
    {
        print("attack");
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1, 0), moveDir, attackRange);
        
        if (hit.transform.gameObject.CompareTag("Crate"))
        {
            print("hit box");
            Destroy(hit.transform.gameObject);
        }
    }

    public void AttackEnd()
    {
        anim.SetBool("IsAttacking", false);
        attacking = false;
        //attackBox.SetActive(false);
    }

    #endregion

    #region death
    void CheckForDeath()
    {
        if (transform.position.y < -10 || Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    #endregion

}
