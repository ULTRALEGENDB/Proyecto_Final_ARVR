using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    // IMPORTANTE: Asegúrate de arrastrar el objeto que tiene el script CambioEscena aquí en cada escena
    public CambioEscena controlador;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica que el Tag del jugador sea exactamente "Player"
        if (other.CompareTag("Player"))
        {
            string escena = SceneManager.GetActiveScene().name;

            if (escena == "Mapa0")
            {
                // Si el controlador no está asignado por error en el inspector, lo buscamos
                if (controlador == null) controlador = FindFirstObjectByType<CambioEscena>();

                controlador.EntrarAlNivelDesdeTren();
            }
            else
            {
                if (controlador == null) controlador = FindFirstObjectByType<CambioEscena>();

                controlador.FinalizarNivel();
            }
        }
    }
}