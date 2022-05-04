using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DB_Manager
{
    public static string username;

    public static bool userExist { get { return !string.IsNullOrEmpty(username); } }

    public static int currperks = 0;
    public static int maxperks = 100;
    public static float sensitivity = 3f;
    public static int playerIconIndex = 0;


    public static void Logout()
    {
        username = null;
        currperks = 0;
        maxperks = 100;
        sensitivity = 3f;
        playerIconIndex = 0;
    }

    public static void LoadLoginData()
    {
        string currperksKey = "currperks" + username;
        currperks = PlayerPrefs.GetInt(currperksKey);

        string maxperksKey = "maxperks" + username;
        maxperks = PlayerPrefs.GetInt(maxperksKey);

        string sensitivityKey = "sensitivity" + username;
        sensitivity = PlayerPrefs.GetFloat(sensitivityKey);

        string playerIconIndexKey = "playerIconIndex" + username;
        playerIconIndex = PlayerPrefs.GetInt(playerIconIndexKey);

        GameStats.username = username;
        GameStats.currPerks = currperks;
        GameStats.maxPerks = maxperks;
        GameStats.sensitivity = sensitivity;
        GameStats.playerIconIndex = playerIconIndex;
    }

    public static void SavePlayerIcon()
    {
        string playerIconIndexKey = "playerIconIndex" + username;
        PlayerPrefs.SetInt(playerIconIndexKey, playerIconIndex);
        GameStats.playerIconIndex = playerIconIndex;
    }


    public static void MakeUser(string username)
    {
        DB_Manager.username = username;
        PlayerPrefs.SetString(username, username);

        string currperksKey = "currperks" + username;
        PlayerPrefs.SetInt(currperksKey, currperks);

        string maxperksKey = "maxperks" + username;
        PlayerPrefs.SetInt(maxperksKey, maxperks);

        string sensitivityKey = "sensitivity" + username;
        PlayerPrefs.SetFloat(sensitivityKey, sensitivity);

        string playerIconIndexKey = "playerIconIndex" + username;
        PlayerPrefs.SetInt(playerIconIndexKey, playerIconIndex);

        GameStats.username = username;
        GameStats.currPerks = currperks;
        GameStats.maxPerks = maxperks;
        GameStats.sensitivity = sensitivity;
        GameStats.playerIconIndex = playerIconIndex;

    }


}
