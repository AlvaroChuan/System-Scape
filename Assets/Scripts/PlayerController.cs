using System.Collections;
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
    private InputAction nextGadgetAction;
    private InputAction previousGadgetAction;
    private InputAction openPadAction;
    private InputAction pauseAction;
    private InputAction zoomAction;

    // Component references
    private Rigidbody rb;
    private CameraController cameraController;
    private Camera mainCamera;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject pad;

    // Movement control variables
    private Vector3 moveDirection;
    private bool move;
    private bool canMove = true;
    private bool canJump = true;
    private bool usingJectpack = false;

    // Usage control variables
    private bool usingDrill = false;
    private bool usingPad = false;
    private Vector3 padOriginalPosition;
    private Vector3 padOriginalRotation;
    private Coroutine padCoroutine;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
        cameraController = Camera.main.transform.parent.GetComponent<CameraController>();
        mainCamera = Camera.main;
        padOriginalPosition = pad.transform.localPosition;
        padOriginalRotation = pad.transform.localRotation.eulerAngles;
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
        jumpAction.canceled += OnEndJump;

        interactAction = inputActions.Player.Interact;
        interactAction.Enable();
        interactAction.started += OnInteract;
        interactAction.canceled += OnEndInteract;

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
            interactAction.started -= OnInteract;
            interactAction.canceled -= OnInteract;
            interactAction.Disable();
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
        Vector3 forward = cameraController.transform.forward;
        Vector3 right = cameraController.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        moveDirection = Vector3.Normalize(forward * input.y + right * input.x);
        move = moveDirection != Vector3.zero;

        // Look input
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        if (lookInput != Vector2.zero) cameraController.RotateCamera(lookInput.x);
        if (zoomAction.ReadValue<float>() != 0) cameraController.ZoomCamera(zoomAction.ReadValue<float>() * 0.1f);

        // Rotate player model towards movement direction
        if (move && canMove) playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, Quaternion.LookRotation(moveDirection), GameManager.instance.RotationSpeed * Time.deltaTime);

        // Handle jump
        canJump = Physics.Raycast(transform.position + new Vector3(0, 0.05f, 0), Vector3.down, 0.15f);
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        Vector3 velocity = rb.linearVelocity;
        velocity.x = move ? moveDirection.x * GameManager.instance.PlayerSpeed : 0;
        velocity.z = move ? moveDirection.z * GameManager.instance.PlayerSpeed : 0;
        rb.linearVelocity = velocity;
        if (usingJectpack) rb.AddForce(Vector3.up * GameManager.instance.JetpackForce, ForceMode.Force);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (canJump) rb.AddForce(Vector3.up * GameManager.instance.JumpForce, ForceMode.Impulse);
        else if (!canJump && GameManager.instance.JetpackEnabled) usingJectpack = true;
    }

    private void OnEndJump(InputAction.CallbackContext context)
    {
        usingJectpack = false; // Stop using jetpack when jump is released
    }

    private void OnInteract(InputAction.CallbackContext context) //TODO
    {
        switch (GameManager.instance.SelectedGadget)
        {
            case 0:
                if (GameManager.instance.drillableMaterials.Count <= 0) return;
                usingDrill = true;
                GameManager.instance.drillableMaterials[0].DrillMaterial();
                break;
        }
    }

    private void OnEndInteract(InputAction.CallbackContext context) //TODO
    {
        switch (GameManager.instance.SelectedGadget)
        {
            case 0:
                usingDrill = false;
                if (GameManager.instance.drillableMaterials.Count > 0) GameManager.instance.drillableMaterials[0].StopDrilling();
                break;
        }
    }

    private void OnNextGadget(InputAction.CallbackContext context)
    {
        if (usingDrill) GameManager.instance.drillableMaterials[0].StopDrilling();
        usingDrill = false;
        GameManager.instance.SwitchGadget(true);
    }

    private void OnPreviousGadget(InputAction.CallbackContext context)
    {
        if (usingDrill) GameManager.instance.drillableMaterials[0].StopDrilling();
        usingDrill = false;
        GameManager.instance.SwitchGadget(false);
    }

    private void OnOpenPad(InputAction.CallbackContext context) //TODO
    {
        usingPad = !usingPad;
        canMove = !canMove;
        if (padCoroutine != null) StopCoroutine(padCoroutine);
        padCoroutine = StartCoroutine(MovePadToTarget(usingPad));
    }

    private void OnPause(InputAction.CallbackContext context) //TODO
    {
        Debug.Log($"Pause {context.ReadValue<float>()}");
    }

    private IEnumerator MovePadToTarget(bool toCamera)
    {
        if (toCamera)
        {
            while (Vector3.Distance(pad.transform.position, mainCamera.transform.position + mainCamera.transform.forward * 0.5f) > 0.01f
                   || Quaternion.Angle(pad.transform.rotation, Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up)) > 1f)
            {
                pad.transform.position = Vector3.Lerp(pad.transform.position, mainCamera.transform.position + mainCamera.transform.forward * 0.5f, 10 * Time.deltaTime);
                pad.transform.rotation = Quaternion.Slerp(pad.transform.rotation, Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up), 10 * Time.deltaTime);
                yield return null;
            }
            pad.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 0.5f;
            pad.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up);
            HUDManager.instance.TogglePadUI(true);
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            HUDManager.instance.TogglePadUI(false);
            while (Vector3.Distance(pad.transform.localPosition, padOriginalPosition) > 0.01f
                   || Quaternion.Angle(pad.transform.localRotation, Quaternion.Euler(padOriginalRotation)) > 1f)
            {
                pad.transform.localPosition = Vector3.Lerp(pad.transform.localPosition, padOriginalPosition, 10 * Time.deltaTime);
                pad.transform.localRotation = Quaternion.Slerp(pad.transform.localRotation, Quaternion.Euler(padOriginalRotation), 10 * Time.deltaTime);
                yield return null;
            }
            pad.transform.localPosition = padOriginalPosition;
            pad.transform.localRotation = Quaternion.Euler(padOriginalRotation);
        }
    }
}
