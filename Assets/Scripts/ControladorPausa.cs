using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorPausa : MonoBehaviour
{
    [Header("Paneles de la Jerarquía")]
    public GameObject canvasPausa;
    public GameObject panelPrincipal;
    public GameObject panelConfiguracion;

    [Header("Ajustes de Posición VR")]
    [Tooltip("Distancia en metros desde la cara del jugador")]
    public float distanciaAlJugador = 1.2f;
    [Tooltip("Ajuste de altura respecto a los ojos (negativo para bajarlo)")]
    public float ajusteAltura = -0.2f;

    private bool estaPausado = false;

    void Start()
    {
        // Al empezar, nos aseguramos que el menú esté oculto y el tiempo corra
        Time.timeScale = 1f;
        if (canvasPausa != null)
            canvasPausa.SetActive(false);
    }

    // El Update se eliminó de aquí porque ahora lo controla el PlayerController 
    // para evitar conflictos de Input y bloqueo de cursor.

    public void Pausar()
    {
        estaPausado = true;
        Time.timeScale = 0f; // Congela el juego

        canvasPausa.SetActive(true);
        panelPrincipal.SetActive(true);
        panelConfiguracion.SetActive(false);

        PosicionarMenuFrente();
    }

    public void Continuar()
    {
        estaPausado = false;
        Time.timeScale = 1f; // Reanuda el juego
        canvasPausa.SetActive(false);

        // Si no estás en VR, recuerda bloquear el cursor de nuevo aquí si es necesario
        // Cursor.lockState = CursorLockMode.Locked;
    }

    public void AbrirConfiguracion()
    {
        panelPrincipal.SetActive(false);
        panelConfiguracion.SetActive(true);
    }

    public void VolverAPausa()
    {
        panelConfiguracion.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    public void IrAlMenuPrincipal()
    {
        Time.timeScale = 1f; // ¡Obligatorio! Si no, el menú principal estará congelado
        SceneManager.LoadScene("MainMenu");
    }

    private void PosicionarMenuFrente()
    {
        // Busca CenterEyeAnchor mediante el Tag MainCamera
        Camera cam = Camera.main != null ? Camera.main : FindObjectOfType<Camera>();

        if (cam != null)
        {
            Transform camTransform = cam.transform;

            // 1. Calcular dirección al frente ignorando la inclinación (Eje Y)
            // Esto evita que el menú salga "tirado en el suelo" si miras hacia abajo
            Vector3 miradaFrente = camTransform.forward;
            miradaFrente.y = 0;
            miradaFrente.Normalize();

            // 2. Calcular posición final (Ojos + Frente * Distancia + Ajuste de Altura)
            Vector3 posicionFinal = camTransform.position + (miradaFrente * distanciaAlJugador);
            posicionFinal.y += ajusteAltura;

            canvasPausa.transform.position = posicionFinal;

            // 3. Hacer que el menú mire al jugador
            // Usamos la posición de la cámara para que el Canvas siempre nos de la cara
            canvasPausa.transform.LookAt(camTransform.position);

            // 4. Corregir rotación (LookAt pone el objeto de espaldas por defecto en Canvas)
            canvasPausa.transform.Rotate(0, 180, 0);
        }
        else
        {
            Debug.LogWarning("ControladorPausa: No se encontró MainCamera para posicionar el menú.");
        }
    }

}