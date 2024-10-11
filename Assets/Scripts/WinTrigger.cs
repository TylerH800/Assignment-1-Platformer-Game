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
        if (Physics2D.OverlapCircle(transform.position, detectRadius, whatIsPlayer))
        {
            animator.SetBool("chestOpen", true);
        }
    }

    public void Win()
    {
        gameManager.DisplayWinScreen();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
