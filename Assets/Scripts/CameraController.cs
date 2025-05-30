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

    private void Awake()
    {
        mainCamera = Camera.main;
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
