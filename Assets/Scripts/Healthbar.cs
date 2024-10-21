using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public Slider slider;
    private PlayerScript player;
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
        slider.value = player.currentHealth;
    }
}
