using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Al presionar la tecla 9:
///   - Abre las puertas de TODOS los vagones activos en escena.
///   - Detiene el rebote en Y desactivando el Animator de cada vagón.
///   - Reduce gradualmente la velocidad del Animator de Directional Light a 0.
///
/// Al iniciar el juego:
///   - Oculta el prefab original asignado en el Inspector (el que
///     InfiniteTrainCorridor duplica y deja en escena).
/// </summary>
public class TrainStopController : MonoBehaviour
{
    [Header("Prefab original")]
    [Tooltip("Arrastra aquí el GameObject del tren original en la escena " +
             "(el que InfiniteTrainCorridor usa como referencia). " +
             "Se ocultará automáticamente al iniciar.")]
    public GameObject originalTrainObject;

    [Header("Animator global")]
    [Tooltip("Animator del Directional Light")]
    public Animator lightAnimator;

    [Header("Puertas")]
    [Tooltip("Desplazamiento en Z para abrir cada puerta")]
    public float doorOpenDistance = 1.6f;

    [Tooltip("Segundos que tardan las puertas en abrirse")]
    public float doorOpenDuration = 1f;

    [Header("Configuración")]
    public float lightSlowDownDuration = 2f;
    public float soundSlowDownDuration = 2f;

    [Header("Audio")]
    public AudioSource train;
    public float trainVolume = 0.7f;
    // ── Estado ──────────────────────────────────────
    private bool stopTriggered = false;
    private Coroutine slowDownCoroutine;

    // ── Unity ───────────────────────────────────────
    private void Start()
    {
        // Reproducir sonido de tren al iniciar
        if (train != null)
        {
            train.Play();
            train.volume = trainVolume;
            Debug.Log("[TrainStopController] Sonido de tren iniciado.");
        }
        else
        {
            Debug.LogWarning("[TrainStopController] No se asignó un AudioSource para el tren.");
        }
        // Ocultar el prefab original al inicio
        if (originalTrainObject != null)
        {
            originalTrainObject.SetActive(false);
            Debug.Log("[TrainStopController] Prefab original ocultado.");
        }
    }

    private void Update()
    {
        if (!stopTriggered && (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9) || OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.One)))
            TriggerTrainStop();
    }

    // ── Lógica principal ────────────────────────────
    private void TriggerTrainStop()
    {
        stopTriggered = true;

        // Obtener TODOS los vagones activos generados por InfiniteTrainCorridor
        List<GameObject> allTrains = FindAllActiveTrains();

        if (allTrains.Count == 0)
        {
            Debug.LogWarning("[TrainStopController] No se encontraron vagones en escena.");
        }

        foreach (GameObject train in allTrains)
        {
            // 1. Detener rebote en Y desactivando el Animator
            Animator trainAnim = train.GetComponent<Animator>();
            if (trainAnim != null)
            {
                // Guardar la posición Y actual antes de que el Animator la suelte
                Vector3 pos = train.transform.localPosition;
                trainAnim.enabled = false;
                // Fijar Y en 0 para que el vagón quede nivelado al detenerse
                train.transform.localPosition = new Vector3(pos.x, 0f, pos.z);
            }

            // 2. Abrir puertas de este vagón
            Transform puertaDer = FindChildByName(train.transform, "puertasTrain");
            Transform puertaIzq = FindChildByName(train.transform, "puertasTrainIzq");

            if (puertaDer != null)
                StartCoroutine(MoveDoor(puertaDer, doorOpenDistance));
            else
                Debug.LogWarning($"[TrainStopController] 'puertasTrain' no encontrada en {train.name}");

            if (puertaIzq != null)
                StartCoroutine(MoveDoor(puertaIzq, -doorOpenDistance));
            else
                Debug.LogWarning($"[TrainStopController] 'puertasTrainIzq' no encontrada en {train.name}");
        }

        Debug.Log($"[TrainStopController] Puertas abiertas en {allTrains.Count} vagón(es).");

        // 3. Frenar Directional Light
        if (lightAnimator != null)
        {
            if (slowDownCoroutine != null) StopCoroutine(slowDownCoroutine);
            slowDownCoroutine = StartCoroutine(SlowDownLight());
            slowDownCoroutine = StartCoroutine(SlowDownSound());
        }


    }

    // ── Búsqueda de vagones ─────────────────────────

    /// <summary>
    /// Devuelve todos los GameObjects TRAINV5_2 que son hijos directos
    /// de un wrapper "TrainSegment_N" (creados por InfiniteTrainCorridor).
    /// </summary>
    private List<GameObject> FindAllActiveTrains()
    {
        List<GameObject> result = new List<GameObject>();
        Animator[] all = FindObjectsOfType<Animator>();

        foreach (Animator anim in all)
        {
            Transform parent = anim.transform.parent;
            if (parent == null || !parent.name.StartsWith("TrainSegment_"))
                continue;

            result.Add(anim.gameObject);
        }

        return result;
    }

    /// <summary>
    /// Busca un hijo por nombre exacto en cualquier nivel de profundidad.
    /// </summary>
    private Transform FindChildByName(Transform root, string childName)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == childName)
                return child;
        }
        return null;
    }

    // ── Movimiento de puertas ───────────────────────
    private IEnumerator MoveDoor(Transform door, float offsetZ)
    {
        Vector3 startPos = door.localPosition;
        Vector3 endPos   = startPos + new Vector3(0f, 0f, offsetZ);
        float elapsed    = 0f;

        while (elapsed < doorOpenDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / doorOpenDuration);
            door.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        door.localPosition = endPos;
    }

    // ── Coroutine luz ───────────────────────────────
    private IEnumerator SlowDownLight()
    {
        float initial = lightAnimator.speed;
        float elapsed = 0f;

        while (elapsed < lightSlowDownDuration)
        {
            elapsed += Time.deltaTime;
            lightAnimator.speed = Mathf.Lerp(initial, 0f, Mathf.Clamp01(elapsed / lightSlowDownDuration));
            yield return null;
        }

        lightAnimator.speed = 0f;
        Debug.Log("[TrainStopController] Directional Light detenido.");
    }
    private IEnumerator SlowDownSound()
    {
        if (train == null) yield break;

        float initialVolume = train.volume;
        float elapsed = 0f;
//ajuste para que el sonido no se detenga de golpe, sino que se desvanezca suavemente
        while (elapsed < soundSlowDownDuration){
            elapsed += Time.deltaTime;
            train.volume = Mathf.Lerp(initialVolume, 0f, Mathf.Clamp01(elapsed / soundSlowDownDuration));
            yield return null;
        }

        train.volume = 0f;
        Debug.Log("[TrainStopController] Sonido de tren detenido."); 
    }

    // ── Reset opcional ──────────────────────────────
    public void ResetStop()
    {
        stopTriggered = false;
        if (lightAnimator != null) lightAnimator.speed = 1f;
    }
}