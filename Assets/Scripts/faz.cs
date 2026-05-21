using UnityEngine;
using UnityEngine.XR; 
using System.Collections.Generic;

public class faz : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject canvasAyudaPC;
    public GameObject canvasAyudaVR;

    void Start()
    {
        DetectarYConfigurarInterfaz();
    }

    void DetectarYConfigurarInterfaz()
    {
        // Verificamos si el XRSettings tiene un dispositivo activo
        // Esto detecta si hay un visor conectado y encendido (Quest, Rift, Index, etc.)
        if (XRSettings.isDeviceActive && XRSettings.loadedDeviceName != "")
        {
            Debug.Log("VR Detectado: Activando interfaz de Realidad Virtual.");
            ActivarVR();
        }
        else
        {
            Debug.Log("VR NO Detectado: Activando interfaz de PC (Pantalla plana).");
            ActivarPC();
        }
    }

    void ActivarVR()
    {
        if (canvasAyudaVR != null) canvasAyudaVR.SetActive(true);
        if (canvasAyudaPC != null) canvasAyudaPC.SetActive(false);
    }

    void ActivarPC()
    {
        if (canvasAyudaPC != null) canvasAyudaPC.SetActive(true);
        if (canvasAyudaVR != null) canvasAyudaVR.SetActive(false);
    }
}