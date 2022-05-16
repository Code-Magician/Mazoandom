using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class MainUIManager : MonoBehaviour
{

    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Text perksText;
    [SerializeField] Text perksLevelText;
    [SerializeField] Image perkslider;
    [SerializeField] Image playerIcon;

    [SerializeField] Text playerProfileHighscore;
    [SerializeField] Text playerProfilePerkLevel;
    [SerializeField] Text playerProfilePerks;
    [SerializeField] Text username;
    [SerializeField] Sprite[] playerIcons;




    private void Start()
    {
        musicSlider.value = GameStats.musicVolume;
        PersistingScript.Instance.ChangeVolume();

        if (DB_Manager.LoggedIn)
        {
            SetPlayerData();
        }
    }




    public void ChangePlayerIcon()
    {
        Sprite curr = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
        GameStats.playerIcon = curr;
        playerIcon.sprite = curr;

        GameStats.playerIconIndex = int.Parse(curr.name) - 1;
        PersistingScript.Instance.SavePlayerIcon();
    }


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
        PersistingScript.Instance.SaveSensitivityandMusicVolume();
    }

    public void MusicVolumeChange()
    {
        GameStats.musicVolume = musicSlider.value;
        PersistingScript.Instance.ChangeVolume();
        PersistingScript.Instance.SaveSensitivityandMusicVolume();
    }



    public void Login()
    {
        // DB_Manager.username = PlayerPrefs.GetString(usernameInput.text, null);
        // if (DB_Manager.userExist)
        // {
        //     DB_Manager.LoadLoginData();
        // }
        // else
        // {
        //     DB_Manager.MakeUser(usernameInput.text);
        // }

        SetPlayerData();
    }


    public void SetPlayerData()
    {
        username.text = DB_Manager.username;
        playerIcon.sprite = playerIcons[GameStats.playerIconIndex];
        GameStats.playerIcon = playerIcon.sprite;

        perksText.text = GameStats.currPerks.ToString("00") + " / " + GameStats.maxPerks.ToString("00");
        perksLevelText.text = ((GameStats.maxPerks - 50) / 50).ToString("00");
        perkslider.fillAmount = GameStats.currPerks / (float)GameStats.maxPerks;

        playerProfileHighscore.text = "Highest Kill : " + GameStats.GetHighScore().ToString("00");
        playerProfilePerkLevel.text = "Experience Level : " + ((GameStats.maxPerks - 50) / 50).ToString("00");
        playerProfilePerks.text = "Current Perks : " + GameStats.currPerks.ToString("00") + " / " + GameStats.maxPerks.ToString("00");

        sensitivitySlider.value = GameStats.sensitivity;
        musicSlider.value = GameStats.musicVolume;
    }


    public void Logout()
    {
        DB_Manager.LogOut();
        SceneManager.LoadScene("RegisterScene");
    }

    public void GoToWebsite()
    {
        Application.OpenURL("http://localhost/Project4/game.php");
    }

}
