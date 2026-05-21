using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderOnFall : MonoBehaviour
{
    public string Mapa1 = "Mapa1";  
    public float limitY = -5f;  

    public GameObject player;

    void Update()
    {
        if (player.transform.position.y <= limitY)
        {
            SceneManager.LoadScene(Mapa1);
        }
    }
}