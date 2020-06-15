using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float jumpForce;
    [SerializeField] private float turnSpeed;
    [Header("Input")]
    [SerializeField] private string horizontalInputAxis;
    [SerializeField] private string verticalInputAxis;
    [SerializeField] private string aimHorizontalInputAxis;
    [SerializeField] private string aimVerticalInputAxis;
    [SerializeField] private KeyCode[] jumpKeys;
    [SerializeField] private string grabInput;
    [Header("Aim Assist")]
    [SerializeField] private float aimAssistSmoothTime;
    [Header("Layermasks")]
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask mouseAimLayerMask;
    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform torso;
    [SerializeField] private Transform legs;
    
    //----- INTERNAL -----//
    public static PlayerMovement instance;
    private PlayerShooting playerShooting;
    private PlayerAimAssist playerAimAssist;
    private Rigidbody rb;
    private Vector2 inputVector;
    private Vector2 aimInputVector;
    private bool jumpFlag;
    [HideInInspector] public bool isStopped;
    private bool canWalkRight = true;
    private Collider mainCollider;
    [HideInInspector] public bool blocked;
    private Transform cameraOrientation;
    private Vector3 aimAssistCurrentVelocity;
    private float timeMoving = 0.0f;
    private Transform pointer;
    private bool mobile = false;

    void Awake ()
    {
        instance = this;
        #if UNITY_ANDROID
        mobile = true;
        #endif
        #if UNITY_IOS
        mobile = true;
        #endif
    }

    void Start ()
    {
        playerShooting = GetComponent<PlayerShooting>();
        playerAimAssist = GetComponentInChildren<PlayerAimAssist>();
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();
        cameraOrientation = CameraBehaviour.instance.orientation;
        pointer = new GameObject("Pointer").transform;
        pointer.SetParent(transform);
        pointer.localPosition = Vector3.zero;
    }

    void Update ()
    {
        // Jump input
        foreach (KeyCode k in jumpKeys)
        {
            if (Input.GetKeyDown(k))
            {
                if (!jumpFlag)
                {
                    jumpFlag = true;
                    StartCoroutine(ResetJumpFlag());
                }
            }
        }

        inputVector = new Vector2(Input.GetAxisRaw(horizontalInputAxis), Input.GetAxisRaw(verticalInputAxis));
        inputVector += new Vector2(CrossPlatformInputManager.GetAxisRaw(horizontalInputAxis), CrossPlatformInputManager.GetAxisRaw(verticalInputAxis));
        if (inputVector.magnitude > 1) inputVector.Normalize();
        aimInputVector = new Vector2(-Input.GetAxisRaw(aimHorizontalInputAxis), Input.GetAxisRaw(aimVerticalInputAxis));
        if (mobile)
        {
            aimInputVector += new Vector2(-CrossPlatformInputManager.GetAxisRaw(aimHorizontalInputAxis), -CrossPlatformInputManager.GetAxisRaw(aimVerticalInputAxis));
        }
        print (aimInputVector);
        if (aimInputVector.magnitude > 1) aimInputVector.Normalize();

        // Blocked
        if (blocked)
        {
            inputVector = Vector2.zero;
        }

        // Animation parameters
        animator.SetFloat("Walk", rb.velocity.magnitude);

        if (isStopped)
        {
            timeMoving = 0.0f;
        }
        else
        {
            timeMoving += Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        // Applying velocity
        Vector3 velocity = cameraOrientation.forward * inputVector.y + cameraOrientation.right * inputVector.x;
        velocity *= speed;
        velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        rb.velocity = Vector3.Lerp(rb.velocity, velocity, acceleration * Time.fixedDeltaTime);

        //----- DEFINING DESIRED ROTATION -----//
        Quaternion desiredTorsoRotation = Quaternion.identity;
        // Aiming with mouse
        if (Input.GetKey(KeyCode.Mouse0) && !mobile)
        {
            if (Physics.Raycast(CameraBehaviour.instance.camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 200, mouseAimLayerMask, QueryTriggerInteraction.Ignore))
            {
                Vector3 point = hit.point;
                point = new Vector3(point.x, transform.position.y, point.z);
                if (Vector3.Distance(transform.position, point) > 0.05f)
                {
                    pointer.LookAt(hit.point);
                    pointer.eulerAngles = new Vector3(0.0f, pointer.eulerAngles.y, 0.0f);
                    desiredTorsoRotation = pointer.rotation;
                }
            }
        }
        // Aiming with joystick
        else if (aimInputVector.magnitude > 0)
        {
            // pointer.LookAt(transform.position + new Vector3(-aimInputVector.x, 0.0f, -aimInputVector.y));
            Vector3 appliedPos = cameraOrientation.forward * -aimInputVector.y + cameraOrientation.right * -aimInputVector.x;
            appliedPos = new Vector3(appliedPos.x, transform.position.y, appliedPos.z);
            appliedPos += transform.position;
            pointer.LookAt(appliedPos);
            pointer.eulerAngles = new Vector3(0.0f, pointer.eulerAngles.y, 0.0f);
            desiredTorsoRotation = pointer.rotation;
        }
        //----- APPLYING ROTATIONS -----//

        // First, player object
        if (isStopped)
        {
            if (desiredTorsoRotation != Quaternion.identity)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredTorsoRotation, turnSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            transform.forward = Vector3.Lerp(transform.forward, new Vector3(velocity.x, 0, velocity.z).normalized, turnSpeed * Time.fixedDeltaTime);
        }

        // And then, torso rotation
        // There's somewhere to aim, so turn
        if (desiredTorsoRotation != Quaternion.identity)
        {
            // torso.rotation = Quaternion.Slerp(torso.rotation, desiredTorsoRotation, turnSpeed * Time.fixedDeltaTime);
            torso.rotation = desiredTorsoRotation;
        }
        // There's nowhere to aim, so reset torso
        else
        {
            torso.localRotation = Quaternion.Slerp(torso.localRotation, Quaternion.identity, turnSpeed * Time.fixedDeltaTime);
        }

        // Flip legs when angle > 180
        if (desiredTorsoRotation != Quaternion.identity)
        {
            if (Quaternion.Angle(transform.rotation, desiredTorsoRotation) > 90)
            {
                // TODO leg animation
                legs.localRotation = Quaternion.Slerp(legs.localRotation, Quaternion.Euler(new Vector3(0, 180, 0)), turnSpeed * Time.fixedDeltaTime);
            }
            else
            {
                // TODO leg animation
                legs.localRotation = Quaternion.Slerp(legs.localRotation, Quaternion.Euler(new Vector3(0, 0, 0)), turnSpeed * Time.fixedDeltaTime);
            }
        }
        else
        {
            // TODO leg animation
            legs.localRotation = Quaternion.Slerp(legs.localRotation, Quaternion.Euler(new Vector3(0, 0, 0)), turnSpeed * Time.fixedDeltaTime);
        }

        // Aim assist
        if (playerShooting.keyPressed)
        {
            if (playerAimAssist.GetAimAssistDirection() != Vector3.zero)
            {
                if (isStopped)
                {
                    transform.forward = Vector3.SmoothDamp(transform.forward, playerAimAssist.GetAimAssistDirection(), ref aimAssistCurrentVelocity, aimAssistSmoothTime * Time.fixedDeltaTime);
                }
                else if (timeMoving <= 1.0f && !Input.GetKey(KeyCode.Mouse0))
                {
                    transform.forward = Vector3.Lerp(transform.forward, playerAimAssist.GetAimAssistDirection(), 10 * Time.fixedDeltaTime);
                }
            }
        }

        // Applying jump
        if (jumpFlag)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            jumpFlag = false;
        }

        // Blocked
        if (blocked)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
        }

        isStopped = (rb.velocity.magnitude < 0.05f);
    }

    bool IsGrounded ()
    {
        return Physics.Raycast(transform.position, -Vector3.up, (mainCollider.bounds.size.y / 2) + 0.1f, groundLayerMask);
    }

    IEnumerator ResetJumpFlag ()
    {
        yield return new WaitForSeconds(0.1f);
        jumpFlag = false;
    }

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.name == "Barrier")
        {
            canWalkRight = false;
        }
    }

    void OnTriggerStay (Collider other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<Enemy>().DetectPlayer(this.gameObject);
        }
    }

    void OnTriggerExit (Collider other)
    {
        if (other.gameObject.name == "Barrier")
        {
            canWalkRight = true;
        }
    }

    void OnCollisionEnter (Collision col)
    {
        if (col.gameObject.tag == "Respawn")
        {
            //TODO particles
            gameObject.SetActive(false);
        }
    }
}