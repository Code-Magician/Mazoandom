using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;

public class PersistingScript : MonoBehaviour
{
    public static PersistingScript Instance
    {
        get;
        set;
    }

    AudioSource backgroundMusic;


    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        backgroundMusic = GetComponent<AudioSource>();
    }


    public void ChangeVolume()
    {
        backgroundMusic.volume = GameStats.musicVolume;
    }



    public void SavePlayerIcon()
    {
        StartCoroutine(PlayerIconForm());
    }

    IEnumerator PlayerIconForm()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", GameStats.username);
        form.AddField("playericonindex", GameStats.playerIconIndex);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/playericonindex.php", form);
        yield return www.SendWebRequest();

        byte[] responseBytes = www.downloadHandler.data;
        string responsefromserver = Encoding.UTF8.GetString(responseBytes);
        string[] responsesArray = responsefromserver.Split('\t');

        if (responsesArray[0] != "0")
        {
            Debug.Log("Error in saving player icon index");
        }
        else
        {
            Debug.Log("Succesfully saved playericonindex");
        }

    }


    public void SaveSensitivityandMusicVolume()
    {
        StartCoroutine(SensitivityAndMusicVolumeForm());
    }

    IEnumerator SensitivityAndMusicVolumeForm()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", GameStats.username);
        form.AddField("sensitivity", GameStats.sensitivity.ToString());
        form.AddField("musicvolume", GameStats.musicVolume.ToString());

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/snm.php", form);
        yield return www.SendWebRequest();

        byte[] responseBytes = www.downloadHandler.data;
        string responsefromserver = Encoding.UTF8.GetString(responseBytes);
        string[] responsesArray = responsefromserver.Split('\t');

        if (responsesArray[0] != "0")
        {
            Debug.Log("Error in saving musicvolume and sensitivity.");
        }
        else
        {
            Debug.Log("Succesfully saved sensitivity and music volume.");
        }

    }

}
