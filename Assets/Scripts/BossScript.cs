using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{

    private Enemy enemySc;
    public GameManager gameManager;
    public GameObject boss;
    // Start is called before the first frame update
    void Start()
    {
        enemySc = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForDeath();
    }

    void CheckForDeath()
    {
        if (enemySc.currentHealth <= 0)
        {
            Debug.Log("win");
            gameManager.DisplayWinScreen();
        }

        if (boss == null)
        {
            print("win");
            gameManager.DisplayWinScreen();
        }
    }
}
