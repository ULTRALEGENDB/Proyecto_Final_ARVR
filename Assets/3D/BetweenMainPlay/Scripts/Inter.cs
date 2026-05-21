using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] private string nextScene = "Mapa0";
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);

        audioSource.Play();
        // Espera opcional para mostrar animación
        yield return new WaitForSeconds(10f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);

        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            yield return null;
        }
    }

}