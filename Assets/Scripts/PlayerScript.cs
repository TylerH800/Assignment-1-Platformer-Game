using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    #region variables and references
    //variables
    public float moveSpeed = 7;
    
    public float jumpForce;
    public bool grounded = true;
    private bool attacking = false;
    private bool jumping = false;

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
    }

    #region basic movement
    void MoveSprite()
    {
        if (!attacking)
        {
            anim.SetFloat("Speed", 0);

            //move the player left and right using the 'a' and 'd' keys, and animate the character
            if (Input.GetKey("a") == true)
            {
                rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
                anim.SetFloat("Speed", 1);
                sr.flipX = true;
            }

            if (Input.GetKey("d") == true)
            {
                rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
                anim.SetFloat("Speed", 1);
                sr.flipX = false;
            }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //when the player hits the ground, they stop jumping and can jump again
        if (collision.gameObject.CompareTag("Ground"))
        {
            anim.SetBool("IsJumping", false);
            anim.SetBool("IsFalling", false);
            grounded = true;
            jumping = false;
        }

    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        //if the player leaves the ground, they are no longer grounded
        if (collision.gameObject.CompareTag("Ground"))
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
            attackBox.SetActive(true);
        }  
        
    }

    public void AttackEnd()
    {
        anim.SetBool("IsAttacking", false);
        attacking = false;
        attackBox.SetActive(false);
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
