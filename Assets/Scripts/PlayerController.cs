using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float flightSpeed = 20f;
    public float boostMultiplier = 3f;
    public float ultraBoostMultiplier = 6f;

    public float maxEnergy = 100f;
    public float energyDrainRate = 20f;
    public float energyRegenRate = 10f;

    public float currentEnergy { get; private set; }

    public bool isBoosting = false;
    public bool isUltraBoosting { get; private set; }

    public Vector2 moveInput;
    public float verticalInput;

    public CharacterController controller;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        currentEnergy = maxEnergy;
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Boost.performed += ctx => isBoosting = true;
        inputActions.Player.Boost.canceled += ctx => isBoosting = false;

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        inputActions.Player.AscendDescend.performed += ctx => verticalInput = ctx.ReadValue<float>();
        inputActions.Player.AscendDescend.canceled += ctx => verticalInput = 0f;

        inputActions.Player.UltraBoost.performed += ctx => isUltraBoosting = true;
        inputActions.Player.UltraBoost.canceled += ctx => isUltraBoosting = false;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Player.Boost.performed -= ctx => isBoosting = true;
        inputActions.Player.Boost.canceled -= ctx => isBoosting = false;

        inputActions.Player.AscendDescend.performed -= ctx => verticalInput = ctx.ReadValue<float>();
        inputActions.Player.AscendDescend.canceled -= ctx => verticalInput = 0f;

        inputActions.Player.UltraBoost.performed -= ctx => isUltraBoosting = true;
        inputActions.Player.UltraBoost.canceled -= ctx => isUltraBoosting = false;

        inputActions.Disable();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
            controller = gameObject.AddComponent<CharacterController>();
    }

    private void Update()
    {
        float currentSpeed = flightSpeed;

        // Handle Ultra Boost logic
        if (isBoosting && isUltraBoosting && currentEnergy > 0)
        {
            currentSpeed *= ultraBoostMultiplier;
            currentEnergy -= energyDrainRate * Time.deltaTime;
            currentEnergy = Mathf.Max(currentEnergy, 0);
        }
        else
        {
            if (isBoosting)
                currentSpeed *= boostMultiplier;

            if (!isUltraBoosting)
            {
                currentEnergy += energyRegenRate * Time.deltaTime;
                currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
            }
        }

        // Get camera-based movement direction
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camRight * moveInput.x + camForward * moveInput.y;

        // Add vertical movement (Space/Ctrl)
        move += Vector3.up * verticalInput;

        move = move.normalized;

        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }
}




