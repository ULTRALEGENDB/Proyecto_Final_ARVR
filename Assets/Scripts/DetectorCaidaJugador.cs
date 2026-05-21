using UnityEngine;
using UnityEngine.SceneManagement;

public class DetectorCaidaJugador : MonoBehaviour
{
    public float limiteCaida = -10f; // Ajusta según la altura de tu mapa

    void Update()
    {
        if (transform.position.y < limiteCaida)
        {
            CargarSiguienteNivel();
        }
    }

    void CargarSiguienteNivel()
    {
        if (CambioEscena.proximoMapa == 1)
        {
            SceneManager.LoadScene("Mapa1");
        }
        else
        {
            SceneManager.LoadScene("Mapa2");
        }
    }
}