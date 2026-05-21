using UnityEngine;

public class MirarJugador : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    public Transform objetivo; // Aquí arrastra tu CenterEyeAnchor del OVRCameraRig
    public float distanciaUmbral = 5f;
    public float velocidadRotacion = 2f;

    private Quaternion rotacionInicial;

    void Start()
    {
        // Guardamos la rotación original para regresar a ella cuando te alejes
        rotacionInicial = transform.rotation;

        // Si no asignaste el objetivo, intentamos buscar la cámara principal
        if (objetivo == null && Camera.main != null)
        {
            objetivo = Camera.main.transform;
        }
    }

    void Update()
    {
        if (objetivo == null) return;

        float distancia = Vector3.Distance(transform.position, objetivo.position);

        if (distancia <= distanciaUmbral)
        {
            // Calculamos la dirección hacia el jugador
            Vector3 direccion = objetivo.position - transform.position;

            // Creamos la rotación deseada (mirar hacia esa dirección)
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);

            // Aplicamos la rotación de forma suavizada (Slerp)
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * velocidadRotacion);
        }
        else
        {
            // Si te alejas, regresa a su estado normal suavemente
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionInicial, Time.deltaTime * velocidadRotacion);
        }
    }

    // Dibujamos el círculo del umbral en el editor para que sea fácil de ajustar
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaUmbral);
    }
}