using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleRandomizer : MonoBehaviour
{
    [Range(0, 10)]
    [SerializeField] private float min = 50.0f;
    [Range(0, 10)]
    [SerializeField] private float max = 50.0f;

    void Start ()
    {
        if (max <= min)
        {
            Debug.LogError("ScaleRandomizer: Max value cannot be less then min.");
            return;
        }
        transform.localScale *= Random.Range(min, max);
    }
}
