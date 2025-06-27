using System;
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
    [SerializeField] private ParticleSystem leftEngine;
    [SerializeField] private ParticleSystem rightEngine;
    [SerializeField] private ParticleSystem leftEngineBrake;
    [SerializeField] private ParticleSystem rightEngineBrake;
    [SerializeField] private GameObject indicator;

    private bool usingPad = false;
    private Rigidbody rb;
    private CameraController cameraController;
    private Camera mainCamera;
    private Vector3 padOriginalPosition;
    private Vector3 padOriginalRotation;
    private Coroutine padCoroutine;
    private string currentPadMenu = "";
    public string nearPlanet = "";
    private Coroutine landingCoroutine;
    private float rotation;
    private float originalTimeToLand = 3f;
    public float TimeToLand;
    private bool soundPlaing = false;

    private void Awake()
    {
        instance = this;
        inputActions = new InputSystem_Actions();
        mainCamera = Camera.main;
        cameraController = mainCamera.transform.parent.GetComponent<CameraController>();
        padOriginalPosition = pad.transform.localPosition;
        padOriginalRotation = pad.transform.localRotation.eulerAngles;
        rb = gameObject.GetComponent<Rigidbody>();
        TimeToLand = originalTimeToLand;
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
        accelerateSpaceshipAction.started += context => {
            leftEngineBrake.Stop();
            rightEngineBrake.Stop();
            leftEngine.Play();
            rightEngine.Play();
        };
        accelerateSpaceshipAction.canceled += context => {
            leftEngine.Stop();
            rightEngine.Stop();
        };

        decelerateSpaceshipAction = inputActions.Player.DecelerateSpaceship;
        decelerateSpaceshipAction.Enable();
        decelerateSpaceshipAction.started += context => {
            leftEngine.Stop();
            rightEngine.Stop();
            leftEngineBrake.Play();
            rightEngineBrake.Play();
        };
        decelerateSpaceshipAction.canceled += context => {
            leftEngineBrake.Stop();
            rightEngineBrake.Stop();
        };

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
        accelerateSpaceshipAction.started -= context => {
            leftEngineBrake.Stop();
            rightEngineBrake.Stop();
            leftEngine.Play();
            rightEngine.Play();
        };
        accelerateSpaceshipAction.canceled -= context => {
            leftEngine.Stop();
            rightEngine.Stop();
        };
        if (decelerateSpaceshipAction != null) decelerateSpaceshipAction.Disable();
        decelerateSpaceshipAction.started -= context => {
            leftEngine.Stop();
            rightEngine.Stop();
            leftEngineBrake.Play();
            rightEngineBrake.Play();
        };
        decelerateSpaceshipAction.canceled -= context => {
            leftEngineBrake.Stop();
            rightEngineBrake.Stop();
        };

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
        if (lookInput != Vector2.zero && Input.GetMouseButton(1) && lookAction.activeControl != null && lookAction.activeControl.device.name.Contains("Mouse")) cameraController.RotateCamera(lookInput.x * 0.33f);
        else if (lookInput != Vector2.zero && lookAction.activeControl != null && !lookAction.activeControl.device.name.Contains("Mouse") && !Input.GetMouseButton(1)) cameraController.RotateCamera(lookInput.x);
        if (zoomAction.ReadValue<float>() != 0) cameraController.ZoomCamera(zoomAction.ReadValue<float>() * 0.1f);
        if (nearPlanet != "")
        {
            indicator.SetActive(true);
            indicator.GetComponent<Indicator>().SetFillAmount((originalTimeToLand - TimeToLand) / originalTimeToLand * 100f);
        }
        else indicator.SetActive(false);
        if (Vector3.Distance(rb.position, Vector3.zero) > 1050) nearPlanet = "Scape";
    }

    private void FixedUpdate()
    {
        if (usingPad)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        if ((accelerateSpaceshipAction.ReadValue<float>() > 0 || decelerateSpaceshipAction.ReadValue<float>() > 0) && !soundPlaing)
        {
            soundPlaing = true;
            SoundManager.instance.PlaySfx(SoundManager.ClipEnum.SpaceShipEngine, true);
        }
        else if (accelerateSpaceshipAction.ReadValue<float>() <= 0 && decelerateSpaceshipAction.ReadValue<float>() <= 0 && soundPlaing)
        {
            soundPlaing = false;
            SoundManager.instance.StopSfx(SoundManager.ClipEnum.SpaceShipEngine);
        }
        rb.AddForce(transform.forward * accelerateSpaceshipAction.ReadValue<float>() * GameManager.instance.SpaceShipAcceleration * 5, ForceMode.Acceleration);
        rb.AddForce(transform.forward * -decelerateSpaceshipAction.ReadValue<float>() * GameManager.instance.SpaceShipAcceleration * 5, ForceMode.Acceleration);
        if (rb.linearVelocity.magnitude > GameManager.instance.MaxSpaceshipSpeed) rb.linearVelocity = rb.linearVelocity.normalized * GameManager.instance.MaxSpaceshipSpeed;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (nearPlanet != "")
        {
            SoundManager.instance.PlaySfx(SoundManager.ClipEnum.LoadingBar);
            if (landingCoroutine != null) StopCoroutine(landingCoroutine);
            landingCoroutine = StartCoroutine(LandOnPlanet());
        }
    }

    private void OnEndInteract(InputAction.CallbackContext context)
    {
        if (landingCoroutine != null) StopCoroutine(landingCoroutine);
        indicator.GetComponent<Indicator>().SetFillAmount(0);
        TimeToLand = originalTimeToLand;
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
        string[] planets = {"Mercum", "Colis", "Phobos", "Regio", "Platum", "Scape"};
        TimeToLand = originalTimeToLand;
        while (TimeToLand > 0 && nearPlanet != "")
        {
            TimeToLand -= Time.deltaTime;
            yield return null;
        }
        if (TimeToLand <= 0 && nearPlanet != "" && nearPlanet != "" && Array.IndexOf(planets, nearPlanet) <= GameManager.instance.LandingGearTier) HUDManager.instance.FadeOut(nearPlanet);
        else if (TimeToLand <= 0 && nearPlanet != "" && Array.IndexOf(planets, nearPlanet) > GameManager.instance.LandingGearTier)
        {
            SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Prohibited, false, 2);
        }
        else if (TimeToLand <= 0 && nearPlanet == "Scape")
        {
            GameManager.instance.AttemptToScape();
        }
    }
}
