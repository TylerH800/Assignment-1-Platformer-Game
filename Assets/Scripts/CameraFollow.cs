using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private GameObject player;
    private Vector3 offset = new Vector3(0, 2, -10);
    

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void LateUpdate()
    {//camera follows player position
        transform.position = player.transform.position + offset;
    }
}
