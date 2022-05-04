using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    [SerializeField] Button loginButton;
    [SerializeField] InputField usernameInput;
    [SerializeField] GameObject loginMenu;



    private void Start()
    {
        musicSlider.value = GameStats.musicVolume;
        PersistingScript.Instance.ChangeVolume();

        if (DB_Manager.userExist)
        {
            SetPlayerData();
            loginMenu.SetActive(false);
        }
    }




    public void ChangePlayerIcon()
    {
        Sprite curr = EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite;
        GameStats.playerIcon = curr;
        playerIcon.sprite = curr;
        DB_Manager.playerIconIndex = int.Parse(curr.name) - 1;
        DB_Manager.SavePlayerIcon();
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
    }

    public void MusicVolumeChange()
    {
        GameStats.musicVolume = musicSlider.value;
        PersistingScript.Instance.ChangeVolume();
    }


    public void IsValidUsername()
    {
        loginButton.interactable = (usernameInput.text.Length >= 8 && usernameInput.text.Length <= 15);
    }

    public void Login()
    {
        DB_Manager.username = PlayerPrefs.GetString(usernameInput.text, null);
        if (DB_Manager.userExist)
        {
            DB_Manager.LoadLoginData();
        }
        else
        {
            DB_Manager.MakeUser(usernameInput.text);
        }

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

        usernameInput.DeactivateInputField();
        usernameInput.text = "";
    }
}
