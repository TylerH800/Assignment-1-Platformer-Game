using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    float time = 0;
    TextMeshProUGUI timer;
    
    void Start()
    {
        timer = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        //updates timer with the elapsed time of the current attempt
        time += Time.deltaTime;
        timer.text = "Time: " + time.ToString("0");
    }
}
