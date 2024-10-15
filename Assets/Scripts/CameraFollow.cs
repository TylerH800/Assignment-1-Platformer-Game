using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject player;
    private Vector3 offset = new Vector3(0, 2, -10);
    public float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    public Transform target;
    

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void LateUpdate()
    {
        //camera follows player position
        if (target != null)
        {            
            transform.position = target.position + offset;
        }
    }
}
