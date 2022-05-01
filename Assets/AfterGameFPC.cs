using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AfterGameFPC : MonoBehaviour
{
    public void GameOver()
    {
        if (GameStats.gameOver)
            SceneManager.LoadScene("MainMenu");
    }
}
