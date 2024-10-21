using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Healthbar : MonoBehaviour
{
    public Slider slider;
    private PlayerScript player;
    public TextMeshProUGUI healthText;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerScript>();
        
        slider.maxValue = player.maxHealth;
        slider.value = player.maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //actual bar has a percentage green depending on health
        slider.value = player.currentHealth;

        //stops health going below zero
        if (player.currentHealth <= 0)
        {
            healthText.color = Color.red;
            player.currentHealth = 0;
        }
        //changes the word display
        healthText.text = "Health: " + player.currentHealth + " / " + player.maxHealth;

       
    }
}
