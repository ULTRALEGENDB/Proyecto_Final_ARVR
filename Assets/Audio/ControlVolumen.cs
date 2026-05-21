using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ControlVolumen : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider sliderVolumen;

    private void OnEnable()
    {
        // Al encenderse el panel, leemos el volumen actual del mixer
        float valorActualdB;
        if (mixer.GetFloat("MasterVol", out valorActualdB))
        {
            // Convertimos de decibelios a valor 0-1 para que el slider se mueva solo
            sliderVolumen.value = Mathf.Pow(10, valorActualdB / 20);
        }
    }

    void Start()
    {
        // Aplicamos el valor que tenga el slider nada más empezar
        CambiarVolumen(sliderVolumen.value);

        // Escuchamos cambios del usuario
        sliderVolumen.onValueChanged.AddListener(CambiarVolumen);
    }

    public void CambiarVolumen(float valor)
    {
        // Convertimos el valor 0-1 del slider a la escala logarítmica de decibelios
        // Usamos 0.0001f para evitar errores con el logaritmo de 0
        float dB = Mathf.Log10(Mathf.Max(0.0001f, valor)) * 20;

        // IMPORTANTE: El nombre "MasterVol" debe coincidir con el que pusiste en el Mixer
        mixer.SetFloat("MasterVol", dB);
    }
}