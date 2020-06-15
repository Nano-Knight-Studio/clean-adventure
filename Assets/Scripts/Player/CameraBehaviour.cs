using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraBehaviour : MonoBehaviour
{
    public static CameraBehaviour instance;
    [SerializeField] private Transform [] targets;
    [Header("Position")]
    [SerializeField] private float smoothTime;
    [SerializeField] private Vector3 offset;
    [Header("Rotation")]
    // [SerializeField] private float lookSpeed;
    [SerializeField] private AnimationCurve rotSpeedOverVelocity;
    [Header("Input")]
    [SerializeField] private string horizontalInput;
    [SerializeField] private string verticalInput;
    [SerializeField] private Vector2 distance;
    [SerializeField] private Animator cameraShakeAnimator;
    [SerializeField] private float inputSmoothTime;
    // INTERNAL
    private Transform pointer;
    [HideInInspector] public Vector2 inputVector;
    [HideInInspector] public Vector2 mouseInputVector;
    [HideInInspector] public Transform orientation;
    private float cooldown = 0.0f;
    private float desiredXRotation = 0.0f;
    private Vector3 currentVelocity;
    private Vector3 inputSpeedCurrentVelocity;
    [HideInInspector] public Camera camera;
    private bool mobile = false;

    void Awake ()
    {
        instance = this;
        pointer = new GameObject("Pointer").transform;
        pointer.SetParent(transform);
        pointer.localPosition = Vector3.zero;
        orientation = new GameObject("Orientation").transform;
        orientation.SetParent(transform);
        orientation.localPosition = Vector3.zero;
        camera = GetComponentInChildren<Camera>();
        #if UNITY_ANDROID
        mobile = true;
        #endif
        #if UNITY_IOS
        mobile = true;
        #endif
    }

    void Update ()
    {
        //----- INPUT -----//
        inputVector = new Vector2(Input.GetAxisRaw(horizontalInput),
                                    Input.GetAxisRaw(verticalInput) * -1);
        if (mobile)
        {
        inputVector += new Vector2(CrossPlatformInputManager.GetAxisRaw(horizontalInput),
                                    -CrossPlatformInputManager.GetAxisRaw(verticalInput) * -1);
        }
        else
        {
            mouseInputVector = new Vector2(Mathf.Lerp(-1.0f, 1.0f, Input.mousePosition.x / Screen.width),
                                        Mathf.Lerp(-1.0f, 1.0f, Input.mousePosition.y / Screen.height)) * 3;
        }
        if (!mobile)
        {
            if (inputVector.magnitude > 0)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (Input.GetMouseButton(0))
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        inputVector += mouseInputVector;
        if (inputVector.magnitude > 1) inputVector.Normalize();
        //-----------------//
    }

    void LateUpdate ()
    {
        camera.transform.localPosition = Vector3.SmoothDamp(camera.transform.localPosition, new Vector3(inputVector.x * distance.x, inputVector.y * distance.y, 0.0f), ref inputSpeedCurrentVelocity, inputSmoothTime * Time.deltaTime);
    }

    void FixedUpdate ()
    {
        //Position
        transform.position = Vector3.SmoothDamp(transform.position, GetAveragePosition() + offset, ref currentVelocity, smoothTime);

        //Resetting orientation
        orientation.eulerAngles = transform.eulerAngles;
        orientation.eulerAngles = new Vector3(0, orientation.eulerAngles.y, 0);
    }

    public void CameraShake(string animationName)
    {
        cameraShakeAnimator.SetTrigger(animationName);
    }

    Vector3 GetAveragePosition ()
    {
        Vector3 average = Vector3.zero;
        int activeTargets = 0;
        for(int i = 0; i < targets.Length; i++)
        {
            if (targets[i].gameObject.activeSelf)
            {
                average += targets[i].position;
                activeTargets++;
            }
        }
        if (activeTargets > 0) average /= activeTargets;
        return average;
    }

    float GetAverageVelocity ()
    {
        float average = 0.0f;
        int activeTargets = 0;
        for(int i = 0; i < targets.Length; i++)
        {
            if (targets[i].gameObject.activeSelf)
            {
                average += targets[i].gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                activeTargets++;
            }
        }
        if (activeTargets > 0) average /= activeTargets;
        return average;
    }
}