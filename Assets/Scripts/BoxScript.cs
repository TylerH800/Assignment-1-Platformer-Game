//some prints or debug.logs have been left in as comments for any future testing or debugging where they might be needed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public int maxHealth = 10;
    int currentHealth;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }

    //takes damage based on a value passed in from the player
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
       // Debug.Log(currentHealth)
;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //print("die");
        Destroy(gameObject);
    }
}
