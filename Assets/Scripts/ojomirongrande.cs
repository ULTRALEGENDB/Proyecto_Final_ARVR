using UnityEngine;

public class SiempreMirarJugador : MonoBehaviour
{
    [Header("Configuración")]
    public Transform objetivo; // Arrastra aquí el CenterEyeAnchor de tu OVRCameraRig
    public float velocidadRotacion = 5f;

    [Header("Restricciones")]
    public bool soloRotarEnY = false; // Activa esto si no quieres que el objeto se incline hacia arriba/abajo

    void Start()
    {
        // Si no asignas nada en el inspector, busca la cámara principal por defecto
        if (objetivo == null && Camera.main != null)
        {
            objetivo = Camera.main.transform;
        }
    }

    void Update()
    {
        if (objetivo == null) return;

        // Calculamos la dirección hacia el jugador
        Vector3 direccion = objetivo.position - transform.position;

        if (soloRotarEnY)
        {
            direccion.y = 0; // Ignoramos la diferencia de altura
        }

        if (direccion != Vector3.zero)
        {
            // Creamos la rotación hacia esa dirección
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);

            // Aplicamos la rotación suavemente
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * velocidadRotacion);
        }
    }
}