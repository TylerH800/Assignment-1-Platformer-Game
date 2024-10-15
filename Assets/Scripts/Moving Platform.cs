using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class MovingPlatform : MonoBehaviour
{
    //variables and references
    public float xDir, yDir, speed, moveTime, waitTime;
    private bool forward = true;
    private bool moving = true;

    private Vector2 direction;
    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        direction = new Vector2(xDir, yDir);
        StartCoroutine(State());
    }

    void Update()
    {
        //the direction is calculated every frame
        direction = new Vector2(xDir, yDir);
    }
    

    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        //movement can only happen if the platform is allowed to move
        if (!moving)
        {
            return;
        }

        //transform.Translate(direction * speed * Time.deltaTime);
        rb.velocity = direction * speed;
    }

    IEnumerator State()
    {
        //makes the platform move for a set time
        yield return new WaitForSeconds(moveTime);

        rb.velocity = Vector2.zero;
        moving = false;
        StartCoroutine(FlipDir());
    }

    IEnumerator FlipDir()
    {
        //flips the platform and makes it stay put for a set time before moving it again
        print("flipping");
        xDir *= -1;
        yDir *= -1;

        yield return new WaitForSeconds(waitTime);
        moving = true;

        StartCoroutine(State());
    }


    

}
