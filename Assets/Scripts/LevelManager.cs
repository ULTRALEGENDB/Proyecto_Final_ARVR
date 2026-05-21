using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Configuraci�n de Objetivos")]
    public int totalObjectsTarget = 10;
    // Cambiado a public para que SaveSystem pueda acceder
    public int currentObjects = 0;

    [Header("Referencias de Escena")]
    public GameObject portal;
    public TextMeshProUGUI textoContador;

    private bool portalActive = false;

    void Start()
    {
        if (portal != null) portal.SetActive(false);
        ActualizarInterfaz();
    }

    public void CollectObject()
    {
        currentObjects++;
        ActualizarInterfaz();

        if (currentObjects >= totalObjectsTarget && !portalActive)
        {
            ActivatePortal();
        }
    }

    // Cambiado a public para refrescar la UI al cargar partida
    public void ActualizarInterfaz()
    {
        if (textoContador != null)
        {
            textoContador.text = $"{currentObjects} / {totalObjectsTarget}";
        }
    }

    void ActivatePortal()
    {
        portalActive = true;
        if (portal != null) portal.SetActive(true);

        if (textoContador != null)
            textoContador.text = "¡Portal Abierto!";
    }
}