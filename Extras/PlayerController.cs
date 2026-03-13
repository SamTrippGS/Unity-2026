using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerControllerTwo : MonoBehaviour
{
    
    [Header("Movement")]
    public float speed = 6f;
    public float TurnSpeed = 12f;
    public float Gravity = -20f;
	public float JumpForce = 8f;

    private CharacterController _cc;
    private Transform CameraTransform;
    private Animator Animator;

    private float _verticalVelocity;
    private bool _isRunning;

    private InputAction _moveAction;
    private InputAction _jumpAction;

    void Start() {

        _cc = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();
        CameraTransform = Camera.main.transform;
        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    public void Jump()
    {
        if (!_cc.isGrounded) return;

        // Physics jump
        _verticalVelocity = JumpForce;

        // Animation trigger
        // if (Animator)
        //     Animator.SetTrigger("Jump");
    }

	void Update()
    {

        Vector2 moveInput = _moveAction.ReadValue<Vector2>();

        if (_jumpAction.WasPressedThisFrame())
        {
            Jump();
        }

        bool isMoving = moveInput.sqrMagnitude > 0.15f * 0.15f;

        // Animator parameters
        if (Animator)
        {
            Animator.SetBool("isMoving", isMoving);
        }

        float currentSpeed = speed;
        // Deadzone
        if (!isMoving)
            moveInput = Vector2.zero;

        // Camera-relative directions
        Vector3 camForward = CameraTransform ? CameraTransform.forward : Vector3.forward;
        Vector3 camRight = CameraTransform ? CameraTransform.right : Vector3.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 desiredMove = camForward * moveInput.y + camRight * moveInput.x;
        if (desiredMove.sqrMagnitude > 1f) desiredMove.Normalize();

        // Rotate to face movement direction
        if (desiredMove.sqrMagnitude > 0f)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredMove, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, TurnSpeed * Time.deltaTime);
        }

        // Gravity
        if (_cc.isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = -2f;

        _verticalVelocity += Gravity * Time.deltaTime;

        Vector3 velocity = desiredMove * currentSpeed;
        velocity.y = _verticalVelocity;

        _cc.Move(velocity * Time.deltaTime);
    }

}
