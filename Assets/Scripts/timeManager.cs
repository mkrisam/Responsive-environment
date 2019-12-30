using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timeManager : MonoBehaviour
{
    float timeScale;
    public Slider timeSlider;

    public void Update()
    {
        timeScale = timeSlider.value;
        Time.timeScale = timeScale;
    }
}
