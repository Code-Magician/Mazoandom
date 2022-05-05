using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


public class RegisterMenu : MonoBehaviour
{
    [SerializeField] GameObject MainCanvas;
    [SerializeField] Text mainCanvasFeedbackText;
    [SerializeField] InputField usernameInput;
    [SerializeField] InputField passwordInput;
    [SerializeField] Button resgisterButton;
    [SerializeField] Text errorText;
    [SerializeField] Text feedbackText;



    private void Update()
    {
        if (resgisterButton.interactable && Input.GetKeyDown(KeyCode.Return))
        {
            CallRegister();
        }
    }


    public void CallRegister()
    {
        StartCoroutine(Register());
    }

    private IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", usernameInput.text);
        form.AddField("password", passwordInput.text);
        form.AddField("currperks", DB_Manager.currPerks);
        form.AddField("maxperks", DB_Manager.maxPerks);
        form.AddField("playericonindex", DB_Manager.playerIconIndex);
        form.AddField("highestkillcount", DB_Manager.highestKillCount);
        form.AddField("sensitivity", DB_Manager.sensitivity.ToString());
        form.AddField("musicvolume", DB_Manager.musicVolume.ToString());



        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/register.php", form);

        yield return www.SendWebRequest();

        // Content of the echo from php script.
        byte[] responseBytes = www.downloadHandler.data;
        string responsefromserver = Encoding.UTF8.GetString(responseBytes);
        string[] responsesArray = responsefromserver.Split('\t');

        if (responsesArray[0] != "0")
        {
            if (!string.IsNullOrEmpty(www.error))
                Debug.Log(www.error.ToString());
            feedbackText.text = responsesArray[1];
        }
        else
        {
            mainCanvasFeedbackText.text = "Successfully Registered. Please Log in to proceed.";
            feedbackText.text = "";
            errorText.text = "";

            MainCanvas.SetActive(true);
            this.gameObject.SetActive(false);

            usernameInput.text = "";
            passwordInput.text = "";
        }
    }


    public void VerifyInputs()
    {
        resgisterButton.interactable = (usernameInput.text.Length >= 8 && passwordInput.text.Length >= 8
                                        && usernameInput.text.Length <= 15 && passwordInput.text.Length <= 15);
    }
}
