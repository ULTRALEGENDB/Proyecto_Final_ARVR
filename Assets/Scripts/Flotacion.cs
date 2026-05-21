using UnityEngine;

public class Flotacion : MonoBehaviour
{
    public float amplitud = 0.5f;  
    public float velocidad = 2f;   

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * velocidad) * amplitud;

        transform.position = new Vector3(posicionInicial.x, nuevaY, posicionInicial.z);
    }
}