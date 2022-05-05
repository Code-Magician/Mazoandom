using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginMenu : MonoBehaviour
{
    [SerializeField] GameObject maincanvas;
    [SerializeField] Text mainMenuFeedbackText;
    [SerializeField] InputField usernameInput;
    [SerializeField] InputField passwordInput;
    [SerializeField] Button loginButton;
    [SerializeField] Text feedbackText;
    [SerializeField] Text errorText;




    private void Update()
    {
        if (loginButton.interactable && Input.GetKeyDown(KeyCode.Return))
        {
            CallLogin();
        }
    }


    public void CallLogin()
    {
        StartCoroutine(Login());
    }

    private IEnumerator Login()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", usernameInput.text);
        form.AddField("password", passwordInput.text);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/sqlconnect/login.php", form);

        yield return www.SendWebRequest();

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
            mainMenuFeedbackText.text = "Welcome " + usernameInput.text;
            feedbackText.text = "";
            errorText.text = "";


            DB_Manager.username = usernameInput.text;
            DB_Manager.currPerks = int.Parse(responsesArray[1]);
            DB_Manager.maxPerks = int.Parse(responsesArray[2]);
            DB_Manager.playerIconIndex = int.Parse(responsesArray[3]);
            DB_Manager.highestKillCount = int.Parse(responsesArray[4]);
            DB_Manager.sensitivity = float.Parse(responsesArray[5]);
            DB_Manager.musicVolume = float.Parse(responsesArray[6]);
            DB_Manager.LoadDataToGame();

            maincanvas.SetActive(true);
            maincanvas.GetComponent<MainMenu>().ToggleButtons();
            this.gameObject.SetActive(false);

            usernameInput.text = "";
            passwordInput.text = "";
        }
    }


    public void VerifyInputs()
    {
        loginButton.interactable = (usernameInput.text.Length >= 8 && passwordInput.text.Length >= 8
                                        && usernameInput.text.Length <= 15 && passwordInput.text.Length <= 15);
    }
}
