using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundDrag;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask ground;
    private bool grounded;


    [Header("Animator")]
    public Animator animator;


    [Header("Idle Timer")]
    public float idle2Interval = 10f;
    private float idleTimer = 0f;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        // Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);

        MyInput();
        SpeedControl();

        if (orientation != null)
        {
            transform.forward = orientation.forward;
        }

        // Handle Drag
        rb.linearDamping = grounded ? groundDrag : 0f;

        HandleMovementAnimation();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void HandleMovementAnimation()
    {
        if (animator == null) return;

        // Walk Animation Logic
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float currentHorizontalSpeed = flatVel.magnitude;

        animator.SetFloat("MovementType", currentHorizontalSpeed);

        // Idle Animation Logics
        if (currentHorizontalSpeed < 0.1f)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idle2Interval)
            {
                animator.SetTrigger("PlayIdle2");
                idleTimer = 0f;
            }
        }
        else
        {
            idleTimer = 0f;
        }
    }
}
