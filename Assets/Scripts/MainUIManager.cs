using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{

    [SerializeField] Slider sensitivitySlider;

    public void Play()
    {
        SceneManager.LoadScene("GameLevel");
    }

    public void Setting()
    {
        // Enable Settings canvas.
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         Application.OpenURL(webplayerQuitURL);
#else
         Application.Quit();
#endif
    }


    public void SensitivityChange()
    {
        GameStats.sensitivity = sensitivitySlider.value;
    }
}
