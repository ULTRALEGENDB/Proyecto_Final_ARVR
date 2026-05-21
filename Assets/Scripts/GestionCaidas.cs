using UnityEngine;

public class GestionCaidas : MonoBehaviour
{
    [Header("Configuración de Teletransporte")]
    public Transform puntoDeInicio; 

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            
            CharacterController cc = other.GetComponent<CharacterController>();

            if (cc != null)
            {  
                cc.enabled = false;

                other.transform.position = puntoDeInicio.position;

                other.transform.rotation = puntoDeInicio.rotation;
               
                cc.enabled = true;

                Debug.Log("Jugador reposicionado con éxito.");
            }
        }
    }
}