using System.Collections.Generic;
using UnityEngine;

public class InfiniteTrainCorridor_Simple : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject trainPrefab; // El prefab del vagón (con su Animator)

    [Header("Configuración")]
    public float segmentLength = 10f;     // Largo de cada vagón en Z
    public int segmentsAhead = 3;         // Vagones visibles por delante
    public int segmentsBehind = 2;        // Vagones visibles por detrás
    public Vector3 corridorOrigin = Vector3.zero; // Punto de inicio (X,Y fijos, Z es el primer vagón)

    [Header("Jugador")]
    public Transform player;

    // Almacenamiento de los wrappers (GameObject vacío que contiene al vagón animado)
    private List<GameObject> activeSegments = new List<GameObject>();
    private int totalSegments;
    private float previousPlayerZ; // Para detectar cuándo mover los vagones

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
            else { Debug.LogError("No Player found"); enabled = false; return; }
        }

        if (trainPrefab == null) { Debug.LogError("No train prefab"); enabled = false; return; }

        totalSegments = segmentsAhead + segmentsBehind + 1;
        GenerateInitialSegments();
        previousPlayerZ = player.position.z;
    }

    void GenerateInitialSegments()
    {
        // Crear todos los vagones desde el índice -segmentsBehind hasta +segmentsAhead
        for (int i = -segmentsBehind; i <= segmentsAhead; i++)
        {
            float worldZ = corridorOrigin.z + i * segmentLength;
            CreateSegmentAtZ(worldZ, i);
        }
    }

    void CreateSegmentAtZ(float z, int debugIndex)
    {
        // Wrapper vacío (sin animador)
        GameObject wrapper = new GameObject($"TrainSegment_{debugIndex}");
        wrapper.transform.position = new Vector3(corridorOrigin.x, corridorOrigin.y, z);
        wrapper.transform.rotation = trainPrefab.transform.rotation;

        // Instanciar el prefab como hijo
        GameObject train = Instantiate(trainPrefab, wrapper.transform);
        train.transform.localPosition = Vector3.zero;
        train.transform.localRotation = Quaternion.identity;

        activeSegments.Add(wrapper);
    }

    void Update()
    {
        if (player == null) return;

        float deltaZ = player.position.z - previousPlayerZ;
        if (Mathf.Abs(deltaZ) >= segmentLength)
        {
            // El jugador ha avanzado o retrocedido un segmento completo
            int steps = Mathf.FloorToInt(deltaZ / segmentLength);
            previousPlayerZ += steps * segmentLength;

            if (steps > 0) // Avanzó hacia adelante (Z+)
                ShiftSegmentsForward(steps);
            else if (steps < 0) // Retrocedió
                ShiftSegmentsBackward(-steps);
        }
    }

    void ShiftSegmentsForward(int steps)
    {
        for (int step = 0; step < steps; step++)
        {
            // Tomar el primer vagón (el más atrás)
            GameObject oldest = activeSegments[0];
            activeSegments.RemoveAt(0);

            // Calcular nueva posición: al final de la cola (detrás del vagón más adelantado)
            GameObject last = activeSegments[activeSegments.Count - 1];
            float newZ = last.transform.position.z + segmentLength;
            oldest.transform.position = new Vector3(corridorOrigin.x, corridorOrigin.y, newZ);

            // Reinsertar al final
            activeSegments.Add(oldest);
        }
    }

    void ShiftSegmentsBackward(int steps)
    {
        for (int step = 0; step < steps; step++)
        {
            // Tomar el último vagón (el más adelante)
            GameObject newest = activeSegments[activeSegments.Count - 1];
            activeSegments.RemoveAt(activeSegments.Count - 1);

            // Calcular nueva posición: delante del vagón más atrás
            GameObject first = activeSegments[0];
            float newZ = first.transform.position.z - segmentLength;
            newest.transform.position = new Vector3(corridorOrigin.x, corridorOrigin.y, newZ);

            // Reinsertar al principio
            activeSegments.Insert(0, newest);
        }
    }

    // Opcional: dibujar gizmos para depurar
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        foreach (var w in activeSegments)
        {
            if (w != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(w.transform.position, new Vector3(3f, 3f, segmentLength));
            }
        }
    }
}