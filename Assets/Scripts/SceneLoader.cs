using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static string entryPointName;
    private static GameObject loadingScreenPrefab;
    private GameObject loadingUIInstance;

    public static void LoadSceneWithEntry(string sceneName, string entryName)
    {
        entryPointName = entryName;

        // Cargar un prefab de pantalla de carga si no lo tenemos aún
        if (loadingScreenPrefab == null)
        {
            loadingScreenPrefab = Resources.Load<GameObject>("LoadingScreen");
        }

        GameObject temp = new GameObject("SceneLoaderTemp");
        SceneLoader loader = temp.AddComponent<SceneLoader>();
        DontDestroyOnLoad(temp);

        loader.StartCoroutine(loader.LoadSceneRoutine(sceneName));
    }

    public static void LoadSceneNoEntry(string sceneName)
    {
        // Cargar un prefab de pantalla de carga si no lo tenemos aún
        if (loadingScreenPrefab == null)
        {
            loadingScreenPrefab = Resources.Load<GameObject>("LoadingScreen");
        }

        GameObject temp = new GameObject("SceneLoaderTemp");
        SceneLoader loader = temp.AddComponent<SceneLoader>();
        DontDestroyOnLoad(temp);

        loader.StartCoroutine(loader.LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // Mostrar la pantalla de carga
        if (loadingScreenPrefab != null)
        {
            loadingUIInstance = Instantiate(loadingScreenPrefab);
            Debug.Log("Instanciando pantalla de carga");
            DontDestroyOnLoad(loadingUIInstance);
        }

        //yield return new WaitForSeconds(0.5f); // opcional, para mostrar la pantalla un momento antes de cargar

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Esperamos a que se cargue casi completamente
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Activamos la escena una vez esté lista
        asyncLoad.allowSceneActivation = true;

        // Esperamos al evento de sceneLoaded (manejado en OnSceneLoaded)
        yield return null;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(MovePlayerNextFrame());
    }

    private IEnumerator MovePlayerNextFrame()
    {
        yield return null;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject entryPoint = GameObject.Find(entryPointName);

        if (player != null && entryPoint != null)
        {
            var controller = player.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            player.transform.SetParent(null);
            player.transform.position = entryPoint.transform.position;

            if (controller != null) controller.enabled = true;

            Debug.Log("Jugador posicionado correctamente en: " + entryPointName);
        }
        else
        {
            Debug.LogWarning("Player o entryPoint no encontrados después de cargar escena.");
        }

        Destroy(gameObject); // destruye el SceneLoaderTemp

        if (loadingUIInstance != null)
        {
            Destroy(loadingUIInstance);
            loadingUIInstance = null;
        }

    }
}