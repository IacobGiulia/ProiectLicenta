using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public bool canMove = true;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 movement;
    private float verticalSpeed;
    private Vector3 velocity;

    [Header("Settings")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    [Header("Physics")]
    [SerializeField] private float checkSphereRadius = 0.3f;
    [SerializeField] private Vector3 checkSphereOffset;
    [SerializeField] private LayerMask groundLayer;

    private ThirdPersonCamera cameraController;
    private CharacterController characterController;
    private Animator animator;

    private Quaternion desiredRotation;
    private Vector3 desiredMovementDir;



    private void Start()
    {
        cameraController = Camera.main.GetComponent<ThirdPersonCamera>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!canMove)
            return;
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;
        bool isRunning = isMoving && Input.GetKey(runKey);

        animator.SetBool("IsRunning", isRunning);
        movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        desiredMovementDir = cameraController.YRotation * movement;

        if (IsGrounded())
            verticalSpeed = -0.5f;
        else
            verticalSpeed += Physics.gravity.y * Time.deltaTime;

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        velocity = desiredMovementDir * currentSpeed;
        velocity.y = verticalSpeed;

        if (isMoving)
        {
            desiredRotation = Quaternion.LookRotation(desiredMovementDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }

        float targetAnimSpeed = isMoving ? 0.5f : 0f;

        animator.SetFloat("MovementSpeed", targetAnimSpeed, 0.2f, Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.TransformPoint(checkSphereOffset), checkSphereRadius, groundLayer);
    }

    public void ResetMovement()
    {
        movement = Vector3.zero;
        desiredMovementDir = Vector3.zero;
        velocity = Vector3.zero;
    }
}
