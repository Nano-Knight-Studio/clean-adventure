using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRandomizer : MonoBehaviour
{
    [SerializeField] private GameObject[] trees;

    void Start ()
    {
        foreach (GameObject obj in trees)
        {
            obj.SetActive(false);
        }
        trees[Random.Range(0, trees.Length)].SetActive(true);
    }
}
