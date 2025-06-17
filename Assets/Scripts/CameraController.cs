using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Player reference")]
    [SerializeField] private Transform player;
    [Header("Camera settings")]
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private float cameraFollowSpeed;
    [SerializeField] private float cameraRotationSpeed;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    private float cameraRotation;
    private RaycastHit hit;
    private Vector3 wishedCameraPosition;
    private Camera mainCamera;
    private Camera HUDCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        HUDCamera = transform.GetChild(1).GetComponent<Camera>();
        ForceAspectRatio();
    }

    private void Start()
    {
        HUDManager.instance.SetTargetCamera(HUDCamera);
    }

    private void Update()
    {
        if (player == null) return;

        // Position lerp
        transform.position = Vector3.Lerp(transform.position, player.position, cameraFollowSpeed * Time.deltaTime);

        // Rotation logic
        if (transform.rotation.eulerAngles.y != cameraRotation)
        {
            Quaternion targetRotation = Quaternion.Euler(0, cameraRotation, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraRotationSpeed * Time.deltaTime);
        }

        // Springarm logic
        Vector3 desiredOffset = transform.rotation * cameraOffset;
        wishedCameraPosition = player.position + desiredOffset;
        if (Physics.Raycast(player.position, desiredOffset.normalized, out hit, desiredOffset.magnitude)) wishedCameraPosition = hit.point - desiredOffset.normalized * 0.1f;
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, wishedCameraPosition, cameraFollowSpeed * 10 * Time.deltaTime);
        mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, Quaternion.LookRotation((player.position + new Vector3(0, 0.5f, 0)) - mainCamera.transform.position, transform.up), cameraRotationSpeed * Time.deltaTime);
    }

    private void ForceAspectRatio()
    {
        float targetAspect = 16f / 9f; // Desired aspect ratio
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            Rect rect = new Rect(0, (1.0f - scaleHeight) / 2.0f, 1.0f, scaleHeight);
            mainCamera.rect = rect;
        }
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = new Rect((1.0f - scaleWidth) / 2.0f, 0, scaleWidth, 1.0f);
            mainCamera.rect = rect;
        }
    }

    public void RotateCamera(float rotation)
    {
        cameraRotation += rotation;
    }

    public void ZoomCamera(float zoomAmount)
    {
        float currentZoom = cameraOffset.magnitude;
        currentZoom = Mathf.Clamp(currentZoom - zoomAmount, minZoom, maxZoom);
        cameraOffset = cameraOffset.normalized * currentZoom;
    }
}
