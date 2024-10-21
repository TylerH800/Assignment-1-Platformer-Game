using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 2, -10);
    public float smoothTime = 0.25f;

    public Transform target;
    

    void LateUpdate()
    {
        //camera follows player position
        if (target != null)
        {            
            transform.position = target.position + offset;
        }
    }
}
