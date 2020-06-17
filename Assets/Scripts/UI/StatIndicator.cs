using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatIndicator : MonoBehaviour
{
    [SerializeField] private float percentage;
    [SerializeField] private float changeSpeed = 1.0f;
    [SerializeField] private Slider slider;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private string measurement;

    void Update ()
    {
        if (image) image.fillAmount = Mathf.Lerp(image.fillAmount, percentage / 100.0f, changeSpeed * Time.deltaTime);
        if (slider) slider.value = Mathf.Lerp(slider.value, percentage / 100.0f, changeSpeed * Time.deltaTime);
    }

    public void SetPercentage(float value)
    {
        percentage = value * 100;
        if (percentageText) percentageText.text = percentage + measurement;
    }
}
