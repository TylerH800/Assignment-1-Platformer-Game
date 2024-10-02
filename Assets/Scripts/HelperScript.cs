using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperScript : MonoBehaviour
{
    private void Start()
    {
        
    }
    public void Test()
    {
        print("Hello World");
    }

    public void FlipObject(bool flip)
    {
        // get the SpriteRenderer component
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();

        if (flip == true)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }

}
