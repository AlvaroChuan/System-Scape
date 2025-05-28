using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Input system
    private InputSystem_Actions inputActions;
    private InputActionReference moveAction;
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

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        accelerateSpaceshipAction = inputActions.Player.Jump;
        accelerateSpaceshipAction.Enable();
        accelerateSpaceshipAction.performed += OnJump;
    }

    private void OnDisable()
    {
        if (accelerateSpaceshipAction != null)
        {
            accelerateSpaceshipAction.performed -= OnJump;
            accelerateSpaceshipAction.Disable();
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("Accelerate Spaceship Action Triggered");
        Debug.Log(context.ReadValue<float>());
    }
}
