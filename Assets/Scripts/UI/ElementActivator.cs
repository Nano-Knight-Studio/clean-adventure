using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementActivator : MonoBehaviour
{
    [Header ("Joystick conditions")]
    public bool appearWhenUsingJoystick = false;
    public bool hideWhenUsingJoystick = false;
    [Header ("Platform conditions")]
    public bool appearOnMobile = false;
    public bool appearOnPC = false;
    public bool appearOnConsole = false;

    void Start ()
    {
        Refresh();
    }

    void Refresh ()
    {
        if (Input.GetJoystickNames().Length > 0 && appearWhenUsingJoystick)
        {
            gameObject.SetActive(true);
            return;
        }

        if (Input.GetJoystickNames().Length > 0 && hideWhenUsingJoystick)
        {
            gameObject.SetActive(false);
            return;
        }

        switch (SystemInfo.deviceType)
        {
            case DeviceType.Handheld:
                gameObject.SetActive(appearOnMobile);
                break;
            case DeviceType.Desktop:
                gameObject.SetActive (appearOnPC);
                break;
            case DeviceType.Console:
                gameObject.SetActive (appearOnConsole);
                break;
        }
    }
}
