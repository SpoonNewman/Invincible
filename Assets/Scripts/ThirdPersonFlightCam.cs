using UnityEngine;

public class ThirdPersonFlightCam : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -10);
    public float followSpeed = 5f;
    public float mouseSensitivity = 3f;
    public float pitchMin = -40f;
    public float pitchMax = 80f;

    public float boostZoomOut = -20f;
    public float zoomLerpSpeed = 5f;

    private float yaw;
    private float pitch;
    private float targetZOffset;

    private Camera cam;
    private PlayerController playerController;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        if (target != null)
        {
            playerController = target.GetComponent<PlayerController>();
            targetZOffset = offset.z;
        }
        else
        {
            Debug.LogError("ThirdPersonFlightCam: No target assigned!");
        }
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

        // Determine input direction
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        Vector3 inputDir = camForward * playerController.moveInput.y + camRight * playerController.moveInput.x;
        inputDir += Vector3.up * playerController.verticalInput;
        inputDir = inputDir.normalized;

        float dot = Vector3.Dot(inputDir, camForward.normalized);
        bool isFlyingTowardCamera = dot < -0.1f;

        // Set base zoom depending on boost state
        float desiredZ = offset.z;

        if (playerController.isUltraBoosting && isFlyingTowardCamera)
        {
            desiredZ = boostZoomOut * 1.5f;
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

        // Create rotation from yaw and pitch
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // Calculate target camera position
        Vector3 targetPos = target.position + rotation * new Vector3(0, offset.y, targetZOffset);

        // Smoothly move camera to target position
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // Instantly rotate camera based on yaw and pitch (no smoothing)
        transform.rotation = rotation;
    }

}

