using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the player attacks the box, the box is destroyed
        if( collision.gameObject.tag == "Attack Box")
        {
            Destroy(gameObject);

        }

    }
}
