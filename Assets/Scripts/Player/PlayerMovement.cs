using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private KeyCode[] jumpKeys;
    [SerializeField] private string grabInput;
    [Header("Ground detection")]
    [SerializeField] private LayerMask layerMask;
    [Header("Animations")]
    [SerializeField] private Animator animator;
    
    //----- INTERNAL -----//
    private PlayerShooting playerShooting;
    private Rigidbody rb;
    private Vector2 inputVector;
    private bool jumpFlag;
    private bool canWalkRight = true;
    private Collider mainCollider;
    [HideInInspector] public bool blocked;
    private Transform cameraOrientation;

    void Start ()
    {
        playerShooting = GetComponent<PlayerShooting>();
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();
        cameraOrientation = CameraBehaviour.instance.orientation;
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
        animator.SetBool("Grounded", IsGrounded());
        animator.SetFloat("Walk", rb.velocity.magnitude);
        print (rb.velocity.magnitude);
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
        if (inputVector.magnitude > 0.1f)
        {
            transform.forward = Vector3.Lerp(transform.forward, new Vector3(velocity.x, 0, velocity.z).normalized, turnSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            transform.forward = Vector3.Lerp(transform.forward, CameraBehaviour.instance.orientation.forward, turnSpeed * Time.deltaTime);
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