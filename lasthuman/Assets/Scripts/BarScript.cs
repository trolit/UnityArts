﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
    private float fillAmount;

    [SerializeField]
    private float lerpSpeed;

    [SerializeField]
    private Image content;

    [SerializeField]
    private Text valueText;

    public float MaxValue { get; set; }

    // we only need "set" in property
    // to set the bar status wherever
    // we are
    public float Value
    {
        set
        {
            string[] tmp = valueText.text.Split(':');
            // not Value but value! - take parameter or it would call itself - stack overflow
            valueText.text = tmp[0] + ": " + value;
            fillAmount = Map(value, 0, MaxValue, 0, 1);
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleBar();
    }

    private void HandleBar()
    {
        // if our fill amount is different that this one in content - update
        if(fillAmount != content.fillAmount)
        {
            // Lerp will move from point to point with speed set in "lerpSpeed" variable
            // multiplied by deltaTime
            content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
        }
    }

    // outMin and outMax are fill amounts 
    private float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        // take current health and change value in scale 0 1
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;

        // (80 - 0) * (1 - 0) / (100 - 0) + 0;
        // 80 * 1 / 100
        // 0.80

        // (78 - 0) * (1 - 0) / (230 - 0) + 0;
        // 78 * 1 / 230
        // 0.339
    }
}
