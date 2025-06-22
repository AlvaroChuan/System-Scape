using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Singleton for easy access
    public static PlayerController instance;

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
    [SerializeField] private SphereCollider drillRangeCollider;
    [SerializeField] private SphereCollider rifleRangeCollider;

    // Movement control variables
    private Vector3 moveDirection;
    private bool move;
    private bool canMove = true;
    private bool canJump = true;
    private bool canAttack = true;
    private bool usingJectpack = false;

    // Usage control variables
    private bool usingDrill = false;
    private bool usingPad = false;
    private string currentPadMenu = "";
    private Transform padOriginalParent;
    private Vector3 padOriginalPosition;
    private Vector3 padOriginalRotation;
    [SerializeField] private Vector3 padTargetScale;
    private Coroutine padCoroutine;
    private bool lockOnTarget = false;
    private Enemy target;
    private bool canUseSpaceship = false;
    private Coroutine mountSpaceshipCoroutine;
    private Animator animator;
    [SerializeField] private GameObject rifle;
    [SerializeField] private GameObject sword;

    private void Awake()
    {
        PlayerController.instance = this;
        inputActions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
        cameraController = Camera.main.transform.parent.GetComponent<CameraController>();
        mainCamera = Camera.main;
        padOriginalPosition = pad.transform.localPosition;
        padOriginalRotation = pad.transform.localRotation.eulerAngles;
        animator = GetComponent<Animator>();
        padOriginalParent = pad.transform.parent;
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
        animator.SetFloat("Speed", move ? 1 : 0);

        // Look input
        if (usingPad) return;
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        if (lookInput != Vector2.zero) cameraController.RotateCamera(lookInput.x);
        if (zoomAction.ReadValue<float>() != 0) cameraController.ZoomCamera(zoomAction.ReadValue<float>() * 0.1f);

        // Rotate player model towards movement direction
        if (!lockOnTarget)
        {
            if (move && canMove) playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, Quaternion.LookRotation(moveDirection), GameManager.instance.RotationSpeed * Time.deltaTime);
        }
        else if (target != null)
        {
            playerModel.transform.rotation = Quaternion.LookRotation(new Vector3(target.transform.position.x, 0, target.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z));
        }

        // Handle jump
        canJump = Physics.Raycast(transform.position + new Vector3(0, 0.05f, 0), Vector3.down, 0.15f);
        animator.SetBool("Grounded", canJump);
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
        if (usingPad) return;
        if (canJump) animator.SetTrigger("Jump");
        if (canJump) rb.AddForce(Vector3.up * GameManager.instance.JumpForce, ForceMode.Impulse);
        else if (!canJump && GameManager.instance.JetpackEnabled) usingJectpack = true;
    }

    private void OnEndJump(InputAction.CallbackContext context)
    {
        if (usingPad) return;
        usingJectpack = false; // Stop using jetpack when jump is released
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (canUseSpaceship)
        {
            if (mountSpaceshipCoroutine != null) StopCoroutine(mountSpaceshipCoroutine);
            mountSpaceshipCoroutine = StartCoroutine(MountSpaceship());
            return;
        }

        switch (GameManager.instance.SelectedGadget)
        {
            case 0:
                if (drillRangeCollider.radius != GameManager.instance.DrillDistance) drillRangeCollider.radius = GameManager.instance.DrillDistance;
                if (GameManager.instance.drillableMaterials.Count <= 0) return;
                usingDrill = true;
                GameManager.instance.drillableMaterials[0].DrillMaterial();
                animator.SetTrigger("Attack");
                break;
            case 1:
                if (canAttack) animator.SetTrigger("Attack");
                if (canAttack) Slice();
                break;
            case 2:
                if (canAttack) StartCoroutine(Shoot());
                break;
        }
    }

    private void OnEndInteract(InputAction.CallbackContext context)
    {
        if (mountSpaceshipCoroutine != null) StopCoroutine(mountSpaceshipCoroutine);
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
        if (usingPad) return;
        if (usingDrill) GameManager.instance.drillableMaterials[0].StopDrilling();
        usingDrill = false;
        GameManager.instance.SwitchGadget(true);
        if (drillRangeCollider.radius != GameManager.instance.DrillDistance) drillRangeCollider.radius = GameManager.instance.DrillDistance;
        if (rifleRangeCollider.radius != GameManager.instance.AimRangeRifle) rifleRangeCollider.radius = GameManager.instance.AimRangeRifle;
    }

    private void OnPreviousGadget(InputAction.CallbackContext context)
    {
        if (usingPad) return;
        if (usingDrill) GameManager.instance.drillableMaterials[0].StopDrilling();
        usingDrill = false;
        GameManager.instance.SwitchGadget(false);
        if (drillRangeCollider.radius != GameManager.instance.DrillDistance) drillRangeCollider.radius = GameManager.instance.DrillDistance;
        if (rifleRangeCollider.radius != GameManager.instance.AimRangeRifle) rifleRangeCollider.radius = GameManager.instance.AimRangeRifle;
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

    public void OpenPad(string menu)
    {
        canMove = false;
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
        canMove = true;
        if (padCoroutine != null) StopCoroutine(padCoroutine);
        padCoroutine = StartCoroutine(MovePadToTarget(false, menu));
        usingPad = false;
        currentPadMenu = "";
    }

    public void AllowSpaceshipMount(bool allow)
    {
        canUseSpaceship = allow;
    }

    private void Slice()
    {
        canAttack = false;
        List<Enemy> enemiesInRange = GameManager.instance.enemiesInMaxRange;
        if (enemiesInRange.Count <= 0) return;
        enemiesInRange.OrderBy(e => Vector3.Distance(transform.position, e.transform.position));
        target = enemiesInRange[0];
        lockOnTarget = true;
        if (target != null && Vector3.Distance(transform.position, target.transform.position) <= GameManager.instance.AimSwordRange) target.TakeDamage(GameManager.instance.SwordDamage);
        lockOnTarget = false;
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator MountSpaceship()
    {
        float timeRemaining = 3f;
        while (timeRemaining > 0 && canUseSpaceship)
        {
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        if (timeRemaining <= 0 && canUseSpaceship) GameManager.instance.LoadScene("Space");
    }

    private IEnumerator Shoot()
    {
        canAttack = false;
        List<Enemy> enemiesInRange = GameManager.instance.enemiesInMaxRange;
        if (enemiesInRange.Count <= 0) yield break;
        enemiesInRange.OrderBy(e => Vector3.Distance(transform.position, e.transform.position));
        target = enemiesInRange[0];
        lockOnTarget = true;
        for (int i = 0; i < GameManager.instance.BulletsPerBurst; i++)
        {
            if (target != null && Vector3.Distance(transform.position, target.transform.position) <= GameManager.instance.AimRangeRifle) target.TakeDamage(GameManager.instance.BulletDamage);
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.1f);
        }
        lockOnTarget = false;
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1f);
        canAttack = true;
    }

    private IEnumerator MovePadToTarget(bool toCamera, string menu)
    {
        if (toCamera)
        {
            pad.transform.parent = mainCamera.transform;
            while (Vector3.Distance(pad.transform.position, mainCamera.transform.position + mainCamera.transform.forward * 0.5f) > 0.01f
                   || Quaternion.Angle(pad.transform.rotation, Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up)) > 1f)
            {
                pad.transform.position = Vector3.Lerp(pad.transform.position, mainCamera.transform.position + mainCamera.transform.forward * 0.5f, 10 * Time.deltaTime);
                pad.transform.rotation = Quaternion.Slerp(pad.transform.rotation, Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up), 10 * Time.deltaTime);
                pad.transform.localScale = Vector3.Lerp(pad.transform.localScale, padTargetScale, 10 * Time.deltaTime);
                yield return null;
            }
            pad.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 0.5f;
            pad.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward, Vector3.up);
            HUDManager.instance.ToggleHUD(true, menu);
            Time.timeScale = 0;
        }
        else
        {
            pad.transform.parent = padOriginalParent;
            Time.timeScale = 1;
            HUDManager.instance.ToggleHUD(false, menu);
            while (Vector3.Distance(pad.transform.localPosition, padOriginalPosition) > 0.01f
                   || Quaternion.Angle(pad.transform.localRotation, Quaternion.Euler(padOriginalRotation)) > 1f)
            {
                pad.transform.localPosition = Vector3.Lerp(pad.transform.localPosition, padOriginalPosition, 10 * Time.deltaTime);
                pad.transform.localRotation = Quaternion.Slerp(pad.transform.localRotation, Quaternion.Euler(padOriginalRotation), 10 * Time.deltaTime);
                pad.transform.localScale = Vector3.Lerp(pad.transform.localScale, Vector3.one, 10 * Time.deltaTime);
                yield return null;
            }
            pad.transform.localPosition = padOriginalPosition;
            pad.transform.localRotation = Quaternion.Euler(padOriginalRotation);
        }
    }

    public void Damage()
    {
        animator.SetTrigger("Hit");
    }

    public void SetAnimTree(bool value)
    {
        animator.SetBool("Sword?", value);
        if (value)
        {
            rifle.SetActive(false);
            sword.SetActive(true);
        }
        else
        {
            rifle.SetActive(true);
            sword.SetActive(false);
        }
    }
}
