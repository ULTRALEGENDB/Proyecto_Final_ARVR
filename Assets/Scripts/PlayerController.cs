using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuracion de Movimiento")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float gravity = 9.81f;
    public float pushPower = 2.0f;

    [Header("Configuracion de Camara")]
    public float mouseSensitivity = 2.0f;
    public float vrLookSensitivity = 80.0f; // Sensibilidad del joystick derecho
    public Transform cameraTransform;       // CenterEyeAnchor

    [Header("Interaccion")]
    public float interactionDistance = 3.0f;
    public LayerMask interactableLayer;
    public GameObject hudText;

    [Header("Referencias de Nivel")]
    public LevelManager levelManager;
    public ControladorPausa scriptPausa;

    private CharacterController controller;
    private float verticalVelocity;
    private float verticalRotation = 0f;

    // ─── Estado interno ───────────────────────────────────────────
    private bool isVR => OVRManager.isHmdPresent;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (!isVR)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (hudText != null) hudText.SetActive(false);
    }

    void Update()
    {
        HandleMovement();
        HandleRotationInput();
        HandleInteractionDetection();
        HandleInputs();

        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * interactionDistance, Color.red);
    }

    void LateUpdate()
    {
        // Rotacion vertical solo en PC (en VR la cabeza mueve la camara sola)
        if (!isVR)
            HandleVerticalRotation();
    }

    // ─── ROTACION ─────────────────────────────────────────────────

    void HandleRotationInput()
    {
        float yaw = 0f;

        if (isVR)
        {
            // Joystick derecho → girar cuerpo horizontalmente
            Vector2 rightStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            yaw = rightStick.x * vrLookSensitivity * Time.deltaTime;
        }
        else
        {
            yaw = Input.GetAxis("Mouse X") * mouseSensitivity;
        }

        transform.Rotate(Vector3.up * yaw);
    }

    void HandleVerticalRotation()
    {
        if (cameraTransform == null) return;

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    // ─── MOVIMIENTO ───────────────────────────────────────────────

    void HandleMovement()
    {
        float moveX, moveZ;
        bool running;

        if (isVR)
        {
            // Joystick izquierdo → movimiento
            Vector2 leftStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            moveX = leftStick.x;
            moveZ = leftStick.y;

            // Thumbstick izquierdo presionado → correr
            running = OVRInput.Get(OVRInput.Button.PrimaryThumbstick);
        }
        else
        {
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");
            running = Input.GetKey(KeyCode.LeftShift);
        }

        float speed = running ? runSpeed : walkSpeed;
        Vector3 move = (transform.right * moveX + transform.forward * moveZ) * speed;

        // Gravedad
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;
        else
            verticalVelocity -= gravity * Time.deltaTime;

        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }

    // ─── DETECCION DE INTERACCION (HUD) ───────────────────────────

    void HandleInteractionDetection()
    {
        RaycastHit hit;
        bool looking = Physics.Raycast(
            cameraTransform.position,
            cameraTransform.forward,
            out hit,
            interactionDistance,
            interactableLayer
        );

        if (looking && (hit.collider.CompareTag("Interactable") || hit.collider.CompareTag("Estrella")))
        {
            if (hudText != null) hudText.SetActive(true);
        }
        else
        {
            if (hudText != null) hudText.SetActive(false);
        }
    }

    // ─── INPUTS DE BOTONES ────────────────────────────────────────

    void HandleInputs()
    {
        // PAUSA → M (PC) | Start / botón menú izquierdo (VR)
        bool pausePressed = Input.GetKeyDown(KeyCode.M)
            || OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch);

        if (pausePressed && scriptPausa != null)
        {
            if (scriptPausa.gameObject.activeSelf)
                scriptPausa.Continuar();
            else
                scriptPausa.Pausar();

            bool paused = scriptPausa.gameObject.activeSelf;
            if (!isVR)
            {
                Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = paused;
            }
        }

        if (Time.timeScale != 0)
        {
            // INTERACTUAR → E (PC) | A / Trigger derecho (VR)
            bool interactPressed = Input.GetKeyDown(KeyCode.E)
                || OVRInput.GetDown(OVRInput.Button.One)                          // Botón A
                || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger);         // Trigger derecho

            if (interactPressed)
                TryInteract();

        
        }
    }

    // ─── LOGICA DE INTERACCION ────────────────────────────────────

    void TryInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionDistance, interactableLayer))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                hit.collider.gameObject.SetActive(false);
                if (hudText != null) hudText.SetActive(false);

                if (levelManager != null)
                    levelManager.CollectObject();
            }
        }
    }

    // ─── EMPUJE DE OBJETOS ────────────────────────────────────────

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;
        if (hit.moveDirection.y < -0.3f) return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.linearVelocity = pushDir * pushPower;
    }
}