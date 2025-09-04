using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Input (arraste do seu Input Actions)")]
    [SerializeField] private InputActionReference moveAction;   // Vector2
    [SerializeField] private InputActionReference lookAction;   // Vector2 (passa pra CameraOrbit)
    [SerializeField] private InputActionReference jumpAction;   // Button
    [SerializeField] private InputActionReference sprintAction; // Button
    [SerializeField] private InputActionReference interactAction; // Button (ex.: E)

    [Header("Referências")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movimento")]
    [SerializeField] private float walkSpeed = 4.0f;
    [SerializeField] private float sprintSpeed = 7.0f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Pulo/Gravidade")]
    [SerializeField] private float jumpHeight = 1.4f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float groundedGravity = -2f;
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask = ~0;

    [Header("Animação (opcional)")]
    [SerializeField] private Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private float currentSpeed;
    private bool isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = 0f;
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        moveAction?.action.Enable();
        lookAction?.action.Enable();
        jumpAction?.action.Enable();
        sprintAction?.action.Enable();
        interactAction?.action.Enable();

        if (interactAction != null)
            interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        moveAction?.action.Disable();
        lookAction?.action.Disable();
        jumpAction?.action.Disable();
        sprintAction?.action.Disable();
        interactAction?.action.Disable();

        if (interactAction != null)
            interactAction.action.performed -= OnInteract;
    }

    private void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJumpAndGravity();
    }

    private void HandleGroundCheck()
    {
        // Usa um SphereCast baixo+raio para estabilidade em rampas
        Vector3 checkPos = groundCheck ? groundCheck.position : (transform.position + Vector3.down * (controller.height * 0.5f - controller.radius + 0.05f));
        isGrounded = Physics.CheckSphere(checkPos, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
        if (isGrounded && velocity.y < 0f)
            velocity.y = groundedGravity; // mantém no chão sem acumular gravidade
    }

    private void HandleMovement()
    {
        Vector2 moveInput = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;

        // direções relativas à câmera
        Vector3 camForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 camRight   = Vector3.ProjectOnPlane(cameraTransform.right,   Vector3.up).normalized;
        Vector3 targetDir  = camForward * moveInput.y + camRight * moveInput.x;
        targetDir = targetDir.normalized;

        // velocidade alvo (caminhar/correr)
        float targetSpeed = (sprintAction != null && sprintAction.action.IsPressed()) ? sprintSpeed : walkSpeed;
        if (targetDir.sqrMagnitude < 0.0001f) targetSpeed = 0f;

        // aceleração suave
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // move no plano XZ
        Vector3 horizontalVel = targetDir * currentSpeed;
        controller.Move(horizontalVel * Time.deltaTime);

        // gira o personagem na direção do movimento
        if (targetDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // parâmetros de animação (opcional)
        if (animator)
        {
            float speedPercent = currentSpeed / sprintSpeed;
            animator.SetFloat("Speed", speedPercent, 0.1f, Time.deltaTime);
            animator.SetBool("Grounded", isGrounded);
        }
    }

    private void HandleJumpAndGravity()
    {
        // pulo
        if (isGrounded && jumpAction != null && jumpAction.action.WasPressedThisFrame())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator) animator.SetTrigger("Jump");
        }

        // gravidade
        velocity.y += gravity * Time.deltaTime;

        // aplica componente vertical
        controller.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        // coloque sua lógica de interação aqui (raycast, trigger, etc.)
        if (animator) animator.SetTrigger("Interact");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
