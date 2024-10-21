using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    public float detectRadius;
    Animator animator;
    public LayerMask whatIsPlayer;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        DetectForPlayer();
    }

    void DetectForPlayer()
    {
        //if the player is close enough, start the chest opening animation
        if (Physics2D.OverlapCircle(transform.position, detectRadius, whatIsPlayer))
        {
            animator.SetBool("chestOpen", true);
        }
    }

    public void Win()
    {
        //makes the cursor visible and moveable
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //called as en event at the end of the chest animation
        gameManager.DisplayWinScreen();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
