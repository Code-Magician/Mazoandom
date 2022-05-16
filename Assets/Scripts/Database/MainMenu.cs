using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // [SerializeField] Text playerDisplay;
    [SerializeField] Button registerButton;
    [SerializeField] Button loginButton;
    [SerializeField] Button playgameButton;


    private void Start()
    {
        // if (DB_Manager.LoggedIn)
        // {
        //     playerDisplay.text = "Player : " + DB_Manager.username + " Score : " + DB_Manager.score;
        // }
        // else
        // {
        //     playerDisplay.text = "No User Logged In";
        // }

        registerButton.interactable = !DB_Manager.LoggedIn;
        loginButton.interactable = !DB_Manager.LoggedIn;
        playgameButton.interactable = DB_Manager.LoggedIn;
    }

    public void GoToPlay()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ToggleButtons()
    {
        registerButton.interactable = !DB_Manager.LoggedIn;
        loginButton.interactable = !DB_Manager.LoggedIn;
        playgameButton.interactable = DB_Manager.LoggedIn;
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

    public void GoToWebsite()
    {
        Application.OpenURL("http://localhost/Project4/game.php");
    }
}
