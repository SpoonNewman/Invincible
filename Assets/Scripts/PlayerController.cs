using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float flightSpeed = 20f;
    public float boostMultiplier = 3f;
    public float ultraBoostMultiplier = 6f;

    [Header("Ultra Boost Energy Settings")]
    public float maxEnergy = 100f;
    public float energyDrainRate = 20f;
    public float energyRegenRate = 10f;

    [Header("Debugging (Read Only)")]
    public bool isBoosting = false;
    public bool isUltraBoosting = false;
    public Vector2 moveInput;
    public float verticalInput;

    [HideInInspector] public CharacterController controller;
    private PlayerInputActions inputActions;
    private float currentEnergy;

    public float CurrentEnergy => currentEnergy;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        currentEnergy = maxEnergy;
    }

    private void OnEnable()
    {
        if (inputActions == null)
            inputActions = new PlayerInputActions();

        inputActions.Enable();

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Boost.performed += ctx => isBoosting = true;
        inputActions.Player.Boost.canceled += ctx => isBoosting = false;
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

        // Handle Boosts
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

        // Camera relative movement
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = (camRight * moveInput.x + camForward * moveInput.y) + (Vector3.up * verticalInput);
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





