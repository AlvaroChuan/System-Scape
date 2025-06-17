using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpaceshipController : MonoBehaviour
{
    public static SpaceshipController instance;

    // Input system
    private InputSystem_Actions inputActions;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction accelerateSpaceshipAction;
    private InputAction decelerateSpaceshipAction;
    private InputAction interactAction;
    private InputAction openPadAction;
    private InputAction pauseAction;
    private InputAction zoomAction;

    [Header("Spaceship Settings")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private GameObject pad;


    private bool usingPad = false;
    private Rigidbody rb;
    private CameraController cameraController;
    private Camera mainCamera;
    private Vector3 padOriginalPosition;
    private Vector3 padOriginalRotation;
    private Coroutine padCoroutine;
    private string currentPadMenu = "";
    public string nearPlanet = "";
    private float actualAcceleration = 0f;
    private Coroutine landingCoroutine;
    private float rotation;

    private void Awake()
    {
        instance = this;
        inputActions = new InputSystem_Actions();
        mainCamera = Camera.main;
        cameraController = mainCamera.transform.parent.GetComponent<CameraController>();
        padOriginalPosition = pad.transform.localPosition;
        padOriginalRotation = pad.transform.localRotation.eulerAngles;
        rb = gameObject.GetComponent<Rigidbody>();
    }

    public void SetPosition()
    {
        GameObject spawnPlanet = GameObject.Find(GameManager.instance.lastPlanet);
        rb.position = spawnPlanet.transform.position - spawnPlanet.transform.right * 55f;
        cameraController.gameObject.transform.position = rb.position;
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

        interactAction = inputActions.Player.Interact;
        interactAction.Enable();
        interactAction.started += OnInteract;
        interactAction.canceled += OnEndInteract;

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

        if (interactAction != null)
        {
            interactAction.started -= OnInteract;
            interactAction.canceled -= OnInteract;
            interactAction.Disable();
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
        if (usingPad) return;
        Vector2 input = moveAction.ReadValue<Vector2>();
        rotation += input.x;
        if (input != Vector2.zero || transform.rotation.eulerAngles.y != rotation) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotation, 0), Time.deltaTime * turnSpeed);
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        if (lookInput != Vector2.zero) cameraController.RotateCamera(lookInput.x);
        if (zoomAction.ReadValue<float>() != 0) cameraController.ZoomCamera(zoomAction.ReadValue<float>() * 0.1f);
    }

    private void FixedUpdate()
    {
        if (usingPad)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        rb.AddForce(transform.forward * accelerateSpaceshipAction.ReadValue<float>() * GameManager.instance.SpaceShipAcceleration * 5, ForceMode.Acceleration);
        rb.AddForce(transform.forward * -decelerateSpaceshipAction.ReadValue<float>() * GameManager.instance.SpaceShipAcceleration * 5, ForceMode.Acceleration);
        if (rb.linearVelocity.magnitude > GameManager.instance.MaxSpaceshipSpeed) rb.linearVelocity = rb.linearVelocity.normalized * GameManager.instance.MaxSpaceshipSpeed;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (nearPlanet != "")
        {
            Debug.Log("Landing on " + nearPlanet);
            if (landingCoroutine != null) StopCoroutine(landingCoroutine);
            landingCoroutine = StartCoroutine(LandOnPlanet());
        }
    }

    private void OnEndInteract(InputAction.CallbackContext context)
    {
        if (landingCoroutine != null) StopCoroutine(landingCoroutine);
    }

    public void OpenPad(string menu)
    {
        currentPadMenu = menu;
        if (usingPad) HUDManager.instance.ToggleHUD(true, menu);
        else
        {
            if (padCoroutine != null) StopCoroutine(padCoroutine);
            padCoroutine = StartCoroutine(MovePadToTarget(true, menu));
        }
        usingPad = true;
    }

    public void ClosePad(string menu)
    {
        if (padCoroutine != null) StopCoroutine(padCoroutine);
        padCoroutine = StartCoroutine(MovePadToTarget(false, menu));
        usingPad = false;
        currentPadMenu = "";
    }

    private void OnOpenPad(InputAction.CallbackContext context) //TODO
    {
        if (currentPadMenu == "Upgrades") ClosePad("Upgrades");
        else OpenPad("Upgrades");
    }

    private void OnPause(InputAction.CallbackContext context) //TODO
    {
        if (currentPadMenu == "Pause") ClosePad("Pause");
        else OpenPad("Pause");
    }

    private IEnumerator MovePadToTarget(bool toCamera, string menu)
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
            HUDManager.instance.ToggleHUD(true, menu);
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
            HUDManager.instance.ToggleHUD(false, menu);
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

    private IEnumerator LandOnPlanet()
    {
        float timeRemaining = 3f;
        while (timeRemaining > 0 && nearPlanet != "")
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        if(timeRemaining <= 0 && nearPlanet != "") GameManager.instance.LoadScene(nearPlanet);
    }
}
