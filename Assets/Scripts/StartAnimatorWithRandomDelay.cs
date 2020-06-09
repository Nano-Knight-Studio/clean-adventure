using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnimatorWithRandomDelay : MonoBehaviour
{
    public float minDelay = 0.0f;
    public float maxDelay = 2.0f;

    IEnumerator Start ()
    {
        GetComponent<Animator>().enabled = false;
        yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        GetComponent<Animator>().enabled = true;
    }
}
