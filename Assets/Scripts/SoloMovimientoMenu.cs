using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SoloMovimientoMenu : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public float walkSpeed = 3.0f;
    public float gravity = 9.81f;

    [Header("Configuracion de Camara (Mouse / VR)")]
    public float mouseSensitivity = 2.0f;
    public float vrLookSensitivity = 80.0f; // Sensibilidad joystick derecho en menú
    public Transform cameraTransform;       // CenterEyeAnchor

    private CharacterController controller;
    private float verticalVelocity;
    private float verticalRotation = 0f;

    private bool isVR => OVRManager.isHmdPresent;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Cursor libre para interactuar con el menú
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
    }

    void HandleRotation()
    {
        // ── Rotacion horizontal ──────────────────────────────────
        float yaw = 0f;

        if (isVR)
        {
            // Joystick derecho → girar en el menú (útil para mirar alrededor)
            Vector2 rightStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            yaw = rightStick.x * vrLookSensitivity * Time.deltaTime;
        }
        else
        {
            yaw = Input.GetAxis("Mouse X") * mouseSensitivity;
        }

        transform.Rotate(Vector3.up * yaw);

        // ── Rotacion vertical (solo PC, en VR la cabeza lo hace sola) ──
        if (!isVR && cameraTransform != null)
        {
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }
    }

    void HandleMovement()
    {
        float moveX, moveZ;

        if (isVR)
        {
            // Joystick izquierdo → moverse por el menú
            Vector2 leftStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            moveX = leftStick.x;
            moveZ = leftStick.y;
        }
        else
        {
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
        }

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (controller.isGrounded)
            verticalVelocity = -2f;
        else
            verticalVelocity -= gravity * Time.deltaTime;

        move.y = verticalVelocity;
        controller.Move(move * walkSpeed * Time.deltaTime);
    }
}