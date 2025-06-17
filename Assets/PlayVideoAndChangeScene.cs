using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PlayVideoAndChangeScene : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;              // Para mostrar el video
    public GameObject videoCanvas;         // Canvas que contiene el RawImage
    public string sceneToLoad;             // Nombre de la escena a cargar
    public Button playButton;              // Botón de UI para iniciar el video

    void Start()
    {
        videoCanvas.SetActive(false);      // Oculta el video hasta que empiece
        playButton.onClick.AddListener(PlayVideo);
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void PlayVideo()
    {
        playButton.gameObject.SetActive(false);  // Oculta el botón
        videoCanvas.SetActive(true);            // Muestra el video
        videoPlayer.Play();                     // Reproduce el video
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(sceneToLoad);    // Cambia de escena al terminar
    }
}