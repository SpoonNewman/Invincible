using UnityEngine;

public class ThirdPersonFlightCam : MonoBehaviour
{
    public Transform target; // The player
    public Vector3 offset = new Vector3(0, 2, -10);
    public float followSpeed = 5f;
    public float boostZoomOut = -15f;
    public float zoomLerpSpeed = 5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float pitchMin = -40f;
    public float pitchMax = 80f;

    private float yaw = 0f;
    private float pitch = 10f;
    private float targetZOffset;
    private Transform cam;
    private PlayerController playerController;

    void Start()
    {
        cam = Camera.main.transform;
        targetZOffset = offset.z;
        playerController = target.GetComponent<PlayerController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // Camera direction
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        // Use input direction instead of velocity for reliability
        Vector3 inputDir = camForward * playerController.moveInput.y + camRight * playerController.moveInput.x;
        inputDir += Vector3.up * playerController.verticalInput;
        inputDir = inputDir.normalized;

        float dot = Vector3.Dot(inputDir, camForward.normalized);
        bool isFlyingTowardCamera = dot < -0.1f;

        // Set base zoom
        float desiredZ = offset.z;

        if (playerController.isUltraBoosting && isFlyingTowardCamera)
        {
            desiredZ = boostZoomOut * 1.5f; // extra zoom for backwards ultra
        }
        else if (playerController.isUltraBoosting)
        {
            desiredZ = boostZoomOut;
        }
        else if (playerController.isBoosting)
        {
            desiredZ = Mathf.Lerp(offset.z, boostZoomOut, 0.5f);
        }

        targetZOffset = Mathf.Lerp(targetZOffset, desiredZ, Time.deltaTime * zoomLerpSpeed);

        // Position camera
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 targetPosition = target.position + rotation * new Vector3(0, offset.y, targetZOffset);

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        cam.LookAt(target.position + Vector3.up * 1.5f);
    }
}

