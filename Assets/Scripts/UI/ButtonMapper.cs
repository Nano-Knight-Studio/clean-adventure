using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMapper : MonoBehaviour
{
    public KeyCode[] keys;

    void Update()
    {
        foreach(KeyCode k in keys)
        {
            if (Input.GetKeyDown(k))
            {
                GetComponent<Button>().onClick.Invoke();
            }
        }
    }
}
