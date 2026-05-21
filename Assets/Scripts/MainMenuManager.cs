using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Jugar()
    {
        // Carga tu escena de juego (ejemplo: "SampleScene")
        SceneManager.LoadScene("BetweenPlayAndTrain");
    }

    public void GuardarPartida()
    {
        // Aqu� ir� tu l�gica de PlayerPrefs o Serializaci�n m�s adelante
        Debug.Log("Partida Guardada");
    }

    public void CargarPartida()
    {
        // Aqu� ir� tu l�gica para leer datos guardados
        Debug.Log("Cargando Partida...");
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("El juego se ha cerrado");
    }
}