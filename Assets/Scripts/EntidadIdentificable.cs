using UnityEngine;

public class EntidadIdentificable : MonoBehaviour
{
    // Este es el ID que se guardará en tu JSON
    public string idUnico;

    // Esto hace que aparezca una opción al hacer clic derecho en el componente
    [ContextMenu("Generar ID Unico")]
    public void GenerarID()
    {
        // Crea un código aleatorio único (ej: 550e8400-e29b-41d4-a716...)
        idUnico = System.Guid.NewGuid().ToString();
        Debug.Log("ID Generado para " + gameObject.name + ": " + idUnico);
    }
}