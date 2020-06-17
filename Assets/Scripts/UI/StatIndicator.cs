using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatIndicator : MonoBehaviour
{
    [SerializeField] private float changeSpeed = 1.0f;
    [Range(0, 100)]
    [SerializeField] private float percentage;
    [SerializeField] private Image image;
    private Vector2 defaultSize;
    RectTransform rectTransform;


    void Start ()
    {
        rectTransform = GetComponent<RectTransform>();
        defaultSize = rectTransform.sizeDelta;
    }

    void Update ()
    {
        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta,
                                                defaultSize * new Vector2(percentage / 100.0f, 1.0f),
                                                changeSpeed *
                                                Time.deltaTime);
        image.fillAmount = Mathf.Lerp(image.fillAmount, percentage / 100.0f, changeSpeed * Time.deltaTime);
    }

    public void SetPercentage(float size)
    {
        percentage = size * 100;
    }
}
