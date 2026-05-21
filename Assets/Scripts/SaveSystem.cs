using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class ObjetoGuardado
{
    public string idUnico;
    public float px, py, pz;
    public bool estaActivo;
}

[System.Serializable]
public class GameData
{
    public float posicionX, posicionY, posicionZ;
    public int estrellasRecogidas; // Nuevo: guarda el progreso del contador
    public List<ObjetoGuardado> objetosEscena = new List<ObjetoGuardado>();
}

public class SaveSystem : MonoBehaviour
{
    private string savePath;
    public GameData data = new GameData();

    [Header("Configuración de Mensajes")]
    public TextMeshProUGUI textoNotificacion;
    public float tiempoVisible = 2.5f;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/savegame.json";
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("DebeCargar") == 1)
        {
            PlayerPrefs.SetInt("DebeCargar", 0);
            CargarPartida();
        }
    }

    public void GuardarPartida()
    {
        data.objetosEscena.Clear();

        // 1. Guardar Jugador
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador)
        {
            data.posicionX = jugador.transform.position.x;
            data.posicionY = jugador.transform.position.y;
            data.posicionZ = jugador.transform.position.z;
        }

        // 2. Guardar Contador de Estrellas
        LevelManager lm = FindObjectOfType<LevelManager>();
        if (lm != null)
        {
            data.estrellasRecogidas = lm.currentObjects;
        }

        // 3. Guardar objetos de la escena
        EntidadIdentificable[] todosLosObjetos = FindObjectsOfType<EntidadIdentificable>(true);
        foreach (var obj in todosLosObjetos)
        {
            ObjetoGuardado info = new ObjetoGuardado();
            info.idUnico = obj.idUnico;
            info.px = obj.transform.position.x;
            info.py = obj.transform.position.y;
            info.pz = obj.transform.position.z;
            info.estaActivo = obj.gameObject.activeSelf;
            data.objetosEscena.Add(info);
        }

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(savePath, json);
        MostrarNotificacion("¡Partida Guardada!");
    }

    public void CargarPartida()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            data = JsonUtility.FromJson<GameData>(json);

            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                PlayerPrefs.SetInt("DebeCargar", 1);
                SceneManager.LoadScene("Mapa1");
                return;
            }

            AplicarDatosALaEscena();
            MostrarNotificacion("¡Partida Cargada!");
        }
    }

    public void AplicarDatosALaEscena()
    {
        // 1. Reposicionar Jugador
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador)
        {
            // Desactivamos momentáneamente para evitar que la física anule el movimiento
            CharacterController cc = jugador.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            jugador.transform.position = new Vector3(data.posicionX, data.posicionY, data.posicionZ);

            if (cc != null) cc.enabled = true;
        }

        // 2. Restaurar Contador y UI
        LevelManager lm = FindObjectOfType<LevelManager>();
        if (lm != null)
        {
            lm.currentObjects = data.estrellasRecogidas;
            lm.ActualizarInterfaz();
        }

        // 3. Restaurar estado de los objetos
        EntidadIdentificable[] objetosEnEscena = FindObjectsOfType<EntidadIdentificable>(true);
        foreach (var infoGuardada in data.objetosEscena)
        {
            foreach (var objReal in objetosEnEscena)
            {
                if (objReal.idUnico == infoGuardada.idUnico)
                {
                    objReal.transform.position = new Vector3(infoGuardada.px, infoGuardada.py, infoGuardada.pz);
                    objReal.gameObject.SetActive(infoGuardada.estaActivo);
                }
            }
        }
    }

    private void MostrarNotificacion(string mensaje)
    {
        if (textoNotificacion != null)
        {
            StopAllCoroutines();
            StartCoroutine(RutinaMensaje(mensaje));
        }
    }

    private IEnumerator RutinaMensaje(string mensaje)
    {
        textoNotificacion.text = mensaje;
        textoNotificacion.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(tiempoVisible);
        textoNotificacion.gameObject.SetActive(false);
    }
}