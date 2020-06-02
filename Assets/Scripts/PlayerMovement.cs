using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public float acceleration;
    public float decceleration;
    public float jumpForce;
    public float turnSpeed;
    [Header("Input")]
    public string horizontalInputAxis;
    public string verticalInputAxis;
    public string jumpInput;
    public string grabInput;
    [Header("Ground detection")]
    public LayerMask layerMask;
    
    //----- INTERNAL -----//
    Rigidbody rb;
    Vector2 inputVector;
    bool jumpFlag;
    bool canWalkRight = true;
    Collider mainCollider;
    [HideInInspector] public bool blocked;
    Animator animator;
    Transform cameraOrientation;

    void Start ()
    {
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();
        cameraOrientation = CameraBehaviour.instance.orientation;
    }

    void Update ()
    {
        // Jump input
        if (Input.GetButtonDown(jumpInput) && IsGrounded())
        {
            if (!jumpFlag)
            {
                jumpFlag = true;
                StartCoroutine(ResetJumpFlag());
            }
        }

        // Blocking right movement
        if (canWalkRight)
        {
            inputVector = new Vector2(Input.GetAxisRaw(horizontalInputAxis), Input.GetAxisRaw(verticalInputAxis)).normalized;
        }
        else
        {
            inputVector = Vector2.Lerp(inputVector, new Vector2(0, Input.GetAxisRaw(verticalInputAxis)).normalized, decceleration * Time.deltaTime);
        }

        // Blocked
        if (blocked)
        {
            inputVector = Vector2.zero;
        }

        // Animation parameters
        if (animator != null)
        {
            animator.SetBool("Grounded", IsGrounded());
            animator.SetFloat("Walk", rb.velocity.magnitude);
        }
    }

    void FixedUpdate()
    {
        // Applying velocity
        Vector3 velocity = cameraOrientation.forward * inputVector.y + cameraOrientation.right * inputVector.x;
        velocity *= speed;
        velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        rb.velocity = Vector3.Lerp(rb.velocity, velocity, acceleration * Time.fixedDeltaTime);
        // rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(inputVector.x * speed, rb.velocity.y, inputVector.y * speed), acceleration * Time.fixedDeltaTime);

        // Rotation
        if (inputVector.magnitude > 0)
        {
            transform.forward = Vector3.Lerp(transform.forward, new Vector3(velocity.x, 0, velocity.z).normalized, turnSpeed * Time.deltaTime);
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
    }

    bool IsGrounded ()
    {
        return Physics.Raycast(transform.position, -Vector3.up, (mainCollider.bounds.size.y / 2) + 0.1f, layerMask);
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