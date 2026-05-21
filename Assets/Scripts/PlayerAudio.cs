using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource footstepSource;
    public Rigidbody rb; // O CharacterController si usas ese

    void Update()
    {
        // Verifica si hay entrada de movimiento (WASD / Flechas)
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        if (isMoving && !footstepSource.isPlaying)
        {
            footstepSource.Play(); // Empieza a sonar
        }
        else if (!isMoving && footstepSource.isPlaying)
        {
            footstepSource.Stop(); // Se detiene cuando dejas de presionar teclas
        }
    }
}