using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MeterScript : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public TextMeshProUGUI resourceCount;

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
        slider.value = value;

        fill.color = gradient.Evaluate (1f) ;
        resourceCount.text = $"{value}/{value}";

    }

    public void SetValue(float value, float maxValue)
    {
        slider.value = value;
		resourceCount.text = $"{value}/{maxValue}";
		fill.color = gradient.Evaluate(slider.normalizedValue);

    }

}
