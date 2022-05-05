using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{



    public void SaveData()
    {
        // DB_Manager.score = score;
        if (DB_Manager.LoggedIn)
        {
            GameStats.SaveData();
            StartCoroutine(Save());
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private IEnumerator Save()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", GameStats.username);
        form.AddField("currperks", GameStats.currPerks);
        form.AddField("maxperks", GameStats.maxPerks);
        form.AddField("playericonindex", GameStats.playerIconIndex);
        form.AddField("highestkillcount", GameStats.highestKillCount);
        form.AddField("sensitivity", GameStats.sensitivity.ToString());
        form.AddField("musicvolume", GameStats.musicVolume.ToString());

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/savegame.php", form);

        yield return www.SendWebRequest();

        byte[] responseBytes = www.downloadHandler.data;
        string responsefromserver = Encoding.UTF8.GetString(responseBytes);
        string[] responsesArray = responsefromserver.Split('\t');


        if (responsesArray[0] != "0")
        {
            if (!string.IsNullOrEmpty(www.error))
                Debug.Log(www.error.ToString());
            else
                Debug.Log(responsefromserver);
        }
        else
        {
            Debug.Log("Successfully Saved Game " + responsefromserver);
        }
    }
}
