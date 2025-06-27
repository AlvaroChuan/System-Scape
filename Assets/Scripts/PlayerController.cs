using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    private bool usingJetpack = false;

    // Usage control variables
    private bool usingDrill = false;
    private bool usingPad = false;
    private bool gettingOxygen = false;
    private string currentPadMenu = "";
    private Transform padOriginalParent;
    private Vector3 padOriginalPosition;
    private Vector3 padOriginalRotation;
    [SerializeField] private Vector3 padTargetScale;
    private Coroutine padCoroutine;
    private bool lockOnTarget = false;
    private GameObject target;
    private bool canUseSpaceship = false;
    private Coroutine mountSpaceshipCoroutine;
    private Animator animator;
    [SerializeField] private GameObject rifle;
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject drill;
    [SerializeField] private ParticleSystem jetpackParticles1;
    [SerializeField] private ParticleSystem jetpackParticles2;
    [SerializeField] private GameObject rifleBurst;
    [SerializeField] private LineRenderer drillLine;
    [SerializeField] private LineRenderer oxygenLine;
    [SerializeField] private GameObject indicator;
    private GameObject oxygenOrigin;
    private float originalTimeMountSpaceship = 3f;
    private float timeMountSpaceship;
    [SerializeField] private GameObject spaceship;
    private bool footstepsEnabled = false;
    private Coroutine footstepsCoroutine;

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
        timeMountSpaceship = originalTimeMountSpaceship;
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
        if (move && canJump && !footstepsEnabled && !usingPad)
        {
            footstepsEnabled = true;
            if (footstepsCoroutine != null) StopCoroutine(footstepsCoroutine);
            footstepsCoroutine = StartCoroutine(FootstepsCoroutine());
        }
        else if ((!move && canJump && footstepsEnabled) || !canJump || usingPad)
        {
            footstepsEnabled = false;
            if (footstepsCoroutine != null) StopCoroutine(footstepsCoroutine);
        }
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        if (lookInput != Vector2.zero && Input.GetMouseButton(1) && lookAction.activeControl != null && lookAction.activeControl.device.name.Contains("Mouse")) cameraController.RotateCamera(lookInput.x * 0.33f);
        else if (lookInput != Vector2.zero && lookAction.activeControl != null && !lookAction.activeControl.device.name.Contains("Mouse") && !Input.GetMouseButton(1)) cameraController.RotateCamera(lookInput.x);
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

        if (GameManager.instance.drillableMaterials.Count > 0 && GameManager.instance.SelectedGadget == 0)
        {
            if (GameManager.instance.drillableMaterials[0] != null) indicator.SetActive(true);
            if (GameManager.instance.drillableMaterials[0] != null) indicator.transform.position = GameManager.instance.drillableMaterials[0].transform.position;
            if (GameManager.instance.drillableMaterials[0] != null) indicator.GetComponent<Indicator>().SetFillAmount(GameManager.instance.drillableMaterials[0].GetDrillTime());
        }
        else if (canUseSpaceship)
        {
            indicator.SetActive(true);
            indicator.transform.position = spaceship.transform.position + Vector3.up * 0.5f; // Position above the spaceship
            indicator.GetComponent<Indicator>().SetFillAmount((originalTimeMountSpaceship - timeMountSpaceship) / originalTimeMountSpaceship * 100f); // Full indicator for spaceship
        }
        else
        {
            indicator.SetActive(false);
        }

        // Handle drill usage
        if (usingDrill && GameManager.instance.drillableMaterials.Count > 0)
        {
            drillLine.enabled = true;
            drillLine.SetPosition(0, drillLine.transform.position);
            drillLine.SetPosition(1, GameManager.instance.drillableMaterials[0].transform.position);
        }
        else drillLine.enabled = false;

        // Handle oxygen regeneration
        if (gettingOxygen)
        {
            oxygenLine.enabled = true;
            oxygenLine.SetPosition(0, oxygenOrigin.transform.position);
            oxygenLine.SetPosition(1, transform.position + Vector3.up * 0.5f);
        }
        else oxygenLine.enabled = false;
    }

    private void FixedUpdate()
    {
        if (!canMove || usingPad)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        Vector3 velocity = rb.linearVelocity;
        velocity.x = move ? moveDirection.x * GameManager.instance.PlayerSpeed : 0;
        velocity.z = move ? moveDirection.z * GameManager.instance.PlayerSpeed : 0;
        rb.linearVelocity = velocity;
        if (usingJetpack) rb.AddForce(Vector3.up * GameManager.instance.JetpackForce, ForceMode.Force);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (usingPad) return;
        if (canJump) animator.SetTrigger("Jump");
        if (canJump) rb.AddForce(Vector3.up * GameManager.instance.JumpForce, ForceMode.Impulse);
        else if (!canJump && GameManager.instance.JetpackEnabled)
        {
            SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Jetpack, true);
            usingJetpack = true;
            jetpackParticles1.Play();
            jetpackParticles2.Play();
        }
    }

    private void OnEndJump(InputAction.CallbackContext context)
    {
        if (usingPad) return;
        usingJetpack = false; // Stop using jetpack when jump is released
        SoundManager.instance.StopSfx(SoundManager.ClipEnum.Jetpack);
        jetpackParticles1.Stop();
        jetpackParticles2.Stop();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (canUseSpaceship)
        {
            SoundManager.instance.PlaySfx(SoundManager.ClipEnum.LoadingBar);
            indicator.GetComponent<Indicator>().SetFillAmount(0);
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
                target = GameManager.instance.drillableMaterials[0].gameObject;
                lockOnTarget = true;
                SoundManager.instance.PlaySfx(SoundManager.ClipEnum.LoadingBar);
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
        if (mountSpaceshipCoroutine != null)
        {
            StopCoroutine(mountSpaceshipCoroutine);
            indicator.GetComponent<Indicator>().SetFillAmount(0);
            timeMountSpaceship = originalTimeMountSpaceship; // Reset mount time
        }
        switch (GameManager.instance.SelectedGadget)
        {
            case 0:
                usingDrill = false;
                lockOnTarget = false;
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
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.OpenPad);
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
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.ClosePad);
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
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Sword);
        canAttack = false;
        List<Enemy> enemiesInRange = GameManager.instance.enemiesInMaxRange;
        if (enemiesInRange.Count <= 0) return;
        enemiesInRange.OrderBy(e => Vector3.Distance(transform.position, e.transform.position));
        target = enemiesInRange[0].gameObject;
        lockOnTarget = true;
        if (target != null && Vector3.Distance(transform.position, target.transform.position) <= GameManager.instance.AimSwordRange) target.GetComponent<Enemy>().TakeDamage(GameManager.instance.SwordDamage);
        StartCoroutine(AttackCooldown());
    }

    public void SetOxygenOrigin(bool state, GameObject origin = null)
    {
        if (state) SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Oxygen);
        oxygenOrigin = origin;
        gettingOxygen = state;
    }

    private IEnumerator MountSpaceship()
    {
        timeMountSpaceship = originalTimeMountSpaceship;
        while (timeMountSpaceship > 0 && canUseSpaceship)
        {
            timeMountSpaceship -= Time.deltaTime;
            yield return null;
        }
        if (timeMountSpaceship <= 0 && canUseSpaceship && GameManager.instance.LandingGearTier > 0) HUDManager.instance.FadeOut("Space");
        else if (timeMountSpaceship <= 0 && canUseSpaceship && GameManager.instance.LandingGearTier <= 0) SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Prohibited, false, 2f);
    }

    private IEnumerator Shoot()
    {
        canAttack = false;
        List<Enemy> enemiesInRange = GameManager.instance.enemiesInMaxRange;
        if (enemiesInRange.Count <= 0) yield break;
        enemiesInRange.OrderBy(e => Vector3.Distance(transform.position, e.transform.position));
        target = enemiesInRange[0].gameObject;
        lockOnTarget = true;
        for (int i = 0; i < GameManager.instance.BulletsPerBurst; i++)
        {
            if (target != null && Vector3.Distance(transform.position, target.transform.position) <= GameManager.instance.AimRangeRifle) target.GetComponent<Enemy>().TakeDamage(GameManager.instance.BulletDamage);
            animator.SetTrigger("Attack");
            rifleBurst.SetActive(true);
            SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Rifle);
            yield return new WaitForSeconds(0.25f);
            rifleBurst.SetActive(false);
            yield return null;
        }
        lockOnTarget = false;
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(1f);
        canAttack = true;
        lockOnTarget = false;
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
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Hit);
        animator.SetTrigger("Hit");
    }

    public void SetAnimTree(int selectedGadget)
    {
        animator.SetBool("Sword?", selectedGadget == 1);
        rifle.SetActive(selectedGadget == 2);
        sword.SetActive(selectedGadget == 1);
        drill.SetActive(selectedGadget == 0);
    }

    private IEnumerator FootstepsCoroutine()
    {
        while (footstepsEnabled)
        {
            SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Footsteps);
            yield return new WaitForSeconds(0.35f);
        }
    }
}
