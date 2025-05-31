using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Input system
    private InputSystem_Actions inputActions;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction accelerateSpaceshipAction;
    private InputAction decelerateSpaceshipAction;
    private InputAction jumpAction;
    private InputAction interactAction;
    private InputAction useGadgetAction;
    private InputAction nextGadgetAction;
    private InputAction previousGadgetAction;
    private InputAction openPadAction;
    private InputAction pauseAction;
    private InputAction zoomAction;

    // Component references
    private Rigidbody rb;
    private GameObject playerModel;
    private CameraController mainCamera;

    // Movement control variables
    private Vector3 moveDirection;
    private bool move;
    private bool canJump = true;
    private bool usingJectpack = false;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main.transform.parent.GetComponent<CameraController>();
        playerModel = transform.GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        moveAction = inputActions.Player.Move;
        moveAction.Enable();

        lookAction = inputActions.Player.Look;
        lookAction.Enable();

        accelerateSpaceshipAction = inputActions.Player.AccelerateSpaceship;
        accelerateSpaceshipAction.Enable();

        decelerateSpaceshipAction = inputActions.Player.DecelerateSpaceship;
        decelerateSpaceshipAction.Enable();

        jumpAction = inputActions.Player.Jump;
        jumpAction.Enable();
        jumpAction.started += OnJump;
        jumpAction.canceled += OnEndJump; // To stop jumping when the button is released

        interactAction = inputActions.Player.Interact;
        interactAction.Enable();
        interactAction.performed += OnInteract;

        useGadgetAction = inputActions.Player.UseGadget;
        useGadgetAction.Enable();
        useGadgetAction.performed += OnUseGadget;

        nextGadgetAction = inputActions.Player.NextGadget;
        nextGadgetAction.Enable();
        nextGadgetAction.performed += OnNextGadget;

        previousGadgetAction = inputActions.Player.PreviousGadget;
        previousGadgetAction.Enable();
        previousGadgetAction.performed += OnPreviousGadget;

        openPadAction = inputActions.Player.OpenPad;
        openPadAction.Enable();
        openPadAction.performed += OnOpenPad;

        pauseAction = inputActions.Player.Pause;
        pauseAction.Enable();
        pauseAction.performed += OnPause;

        zoomAction = inputActions.Player.ZoomInOut;
        zoomAction.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.Disable();
        if (lookAction != null) lookAction.Disable();
        if (accelerateSpaceshipAction != null) accelerateSpaceshipAction.Disable();
        if (decelerateSpaceshipAction != null) decelerateSpaceshipAction.Disable();

        if (jumpAction != null)
        {
            jumpAction.started -= OnJump;
            jumpAction.canceled -= OnEndJump;
            jumpAction.Disable();
        }

        if (interactAction != null)
        {
            interactAction.performed -= OnInteract;
            interactAction.Disable();
        }

        if (useGadgetAction != null)
        {
            useGadgetAction.performed -= OnUseGadget;
            useGadgetAction.Disable();
        }

        if (nextGadgetAction != null)
        {
            nextGadgetAction.performed -= OnNextGadget;
            nextGadgetAction.Disable();
        }

        if (previousGadgetAction != null)
        {
            previousGadgetAction.performed -= OnPreviousGadget;
            previousGadgetAction.Disable();
        }

        if (openPadAction != null)
        {
            openPadAction.performed -= OnOpenPad;
            openPadAction.Disable();
        }

        if (pauseAction != null)
        {
            pauseAction.performed -= OnPause;
            pauseAction.Disable();
        }

        if (zoomAction != null) zoomAction.Disable();
    }

    private void Update()
    {
        // Movement input
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        moveDirection = Vector3.Normalize(forward * input.y + right * input.x);
        move = moveDirection != Vector3.zero;

        // Look input
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        if (lookInput != Vector2.zero) mainCamera.RotateCamera(lookInput.x);
        if (zoomAction.ReadValue<float>() != 0) mainCamera.ZoomCamera(zoomAction.ReadValue<float>() * 0.1f);

        // Rotate player model towards movement direction
        if (move) playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, Quaternion.LookRotation(moveDirection), GameManager.instance.RotationSpeed * Time.deltaTime);

        // Handle jump
        canJump = Physics.Raycast(transform.position + new Vector3(0, 0.05f, 0), Vector3.down, 0.15f);
    }

    private void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = move? moveDirection.x * GameManager.instance.PlayerSpeed : 0;
        velocity.z = move? moveDirection.z * GameManager.instance.PlayerSpeed : 0;
        rb.linearVelocity = velocity;
        if (usingJectpack) rb.AddForce(Vector3.up * GameManager.instance.JetpackForce, ForceMode.Force);
    }

    private void OnJump(InputAction.CallbackContext context) //TODO
    {
        if (canJump) rb.AddForce(Vector3.up * GameManager.instance.JumpForce, ForceMode.Impulse);
        else if (!canJump && true) usingJectpack = true; // Placeholder for jetpack logic
    }

    private void OnEndJump(InputAction.CallbackContext context)
    {
        usingJectpack = false; // Stop using jetpack when jump is released
    }

    private void OnInteract(InputAction.CallbackContext context) //TODO
    {
        Debug.Log($"Interact {context.ReadValue<float>()}");
    }

    private void OnUseGadget(InputAction.CallbackContext context) //TODO
    {
        Debug.Log($"Use Gadget {context.ReadValue<float>()}");
        GameManager.instance.DamagePlayer(10);
    }

    private void OnNextGadget(InputAction.CallbackContext context) //TODO
    {
        Debug.Log($"Next Gadget {context.ReadValue<float>()}");
    }

    private void OnPreviousGadget(InputAction.CallbackContext context) //TODO
    {
        Debug.Log($"Previous Gadget {context.ReadValue<float>()}");
    }

    private void OnOpenPad(InputAction.CallbackContext context) //TODO
    {
        Debug.Log($"Open Pad {context.ReadValue<float>()}");
    }

    private void OnPause(InputAction.CallbackContext context) //TODO
    {
        Debug.Log($"Pause {context.ReadValue<float>()}");
    }
}
