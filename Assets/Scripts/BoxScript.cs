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
