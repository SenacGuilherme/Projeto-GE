using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform followTarget; // normalmente o Player
    [SerializeField] private Transform pivot;        // um pivot vazio como filho do player (altura do ombro/cabeça)
    [SerializeField] private Transform cameraHolder; // objeto com a Camera como filho (offset)

    [Header("Input")]
    [SerializeField] private InputActionReference lookAction; // Vector2 (mouse/RS)
    [SerializeField] private InputActionReference zoomAction; // Vector2 (Scroll Y) - opcional

    [Header("Orbit")]
    [SerializeField] private float yawSensitivity = 180f;
    [SerializeField] private float pitchSensitivity = 120f;
    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 65f;

    [Header("Zoom")]
    [SerializeField] private float distance = 4.0f;
    [SerializeField] private float minDistance = 2.0f;
    [SerializeField] private float maxDistance = 6.5f;
    [SerializeField] private float zoomSpeed = 4.0f;

    private float yaw;
    private float pitch;

    private void OnEnable()
    {
        lookAction?.action.Enable();
        zoomAction?.action.Enable();
    }

    private void OnDisable()
    {
        lookAction?.action.Disable();
        zoomAction?.action.Disable();
    }

    private void LateUpdate()
    {
        if (!followTarget || !pivot || !cameraHolder) return;

        // input de câmera
        Vector2 look = lookAction != null ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;
        yaw   += look.x * yawSensitivity * Time.deltaTime;
        pitch -= look.y * pitchSensitivity * Time.deltaTime;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

        // aplica rotação no pivot (pitch) e no holder root (yaw)
        pivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.position = followTarget.position;       // raiz acompanha o player
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // zoom pelo scroll (opcional)
        if (zoomAction != null)
        {
            float scrollY = zoomAction.action.ReadValue<Vector2>().y;
            distance = Mathf.Clamp(distance - scrollY * zoomSpeed * Time.deltaTime, minDistance, maxDistance);
        }

        // posiciona a camera ao longo do -Z local do pivot
        cameraHolder.localPosition = new Vector3(0f, 0f, -distance);
    }
}
