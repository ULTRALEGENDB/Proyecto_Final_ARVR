using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    // static es vital para que sobreviva al cambio de escenas
    public static int proximoMapa = 1;

    public void EmpezarJuego()
    {
        proximoMapa = 1; // Reset por seguridad al empezar
        SceneManager.LoadScene("BetweenPlayAndTrain");
    }

    public void IrAlTren() => SceneManager.LoadScene("Mapa0");

    public void EntrarAlNivelDesdeTren()
    {
        Debug.Log("Saliendo del tren. Valor de proximoMapa: " + proximoMapa);

        if (proximoMapa == 1)
        {
            SceneManager.LoadScene("Mapa1");
        }
        else if (proximoMapa == 2)
        {
            SceneManager.LoadScene("Mapa2");
        }
    }

    public void FinalizarNivel()
    {
        string actual = SceneManager.GetActiveScene().name;
        Debug.Log("Finalizando nivel: " + actual);

        if (actual == "Mapa1")
        {
            proximoMapa = 2; // Aquí es donde le decimos que el siguiente es el 2
            Debug.Log("Progreso actualizado. Siguiente destino: Mapa2");
            SceneManager.LoadScene("Mapa0");
        }
        else if (actual == "Mapa2")
        {
            proximoMapa = 1;
            SceneManager.LoadScene("MainMenu");
        }
    }
}