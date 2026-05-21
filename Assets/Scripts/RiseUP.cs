using UnityEngine;
using System.Collections;

public class RiseUP : MonoBehaviour
{
    [Header("Configuración de VR")]
    public Transform trackingSpace;
    public OVRScreenFade screenFade;

    [Header("Configuración de Animación")]
    public float duracion = 3.5f;
    public float inclinacionInicialX = -35f;
    public AnimationCurve curvaMovimiento = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();

        if (trackingSpace != null && screenFade != null)
        {
            StartCoroutine(SecuenciaDespertarSegura());
        }
    }

    IEnumerator SecuenciaDespertarSegura()
    {
        // 1. Esperamos al motor de VR
        yield return new WaitForEndOfFrame();

        if (playerController != null) playerController.enabled = false;

        float alturaRealHMD = screenFade.transform.localPosition.y;
        if (alturaRealHMD <= 0) alturaRealHMD = 1.65f;

        float tiempo = 0;
        Vector3 posInicialTS = trackingSpace.localPosition;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / duracion;
            float valorCurva = curvaMovimiento.Evaluate(progreso);

            // MOVIMIENTO VERTICAL
            float alturaDeseada = Mathf.Lerp(0.1f, alturaRealHMD, valorCurva);
            float offsetOffsetY = alturaDeseada - alturaRealHMD;

            // ROTACIÓN (Aplicada al TrackingSpace para evitar conflictos)
            float rotX = Mathf.Lerp(inclinacionInicialX, 0f, valorCurva);

            // Aplicamos ambos cambios al TrackingSpace
            trackingSpace.localPosition = new Vector3(posInicialTS.x, offsetOffsetY, posInicialTS.z);
            trackingSpace.localRotation = Quaternion.Euler(rotX, 0, 0);

            yield return null;
        }

        // RESTABLECER 
        trackingSpace.localPosition = new Vector3(posInicialTS.x, 0, posInicialTS.z);
        trackingSpace.localRotation = Quaternion.identity;

        if (playerController != null) playerController.enabled = true;
    }
}