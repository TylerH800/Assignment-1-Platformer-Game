using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RisingFlatform : MonoBehaviour
{
    public GameObject risingPlatform;
    public Transform risingPlatformSpawn;
    public float repeatRate;
    private float startDelay = 5;
    private Transform spawnPos1;
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnPlatforms", startDelay, repeatRate);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnPlatforms()
    {
        
    }
}
