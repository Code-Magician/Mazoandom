using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DB_Manager
{
    public static string username;
    public static int currPerks = 0;
    public static int maxPerks = 100;
    public static int playerIconIndex = 0;
    public static int highestKillCount = 0;
    public static float sensitivity = 3f;
    public static float musicVolume = 3f;



    public static bool LoggedIn { get { return !string.IsNullOrEmpty(username); } }
    public static void LogOut()
    {
        username = null;
        currPerks = 0;
        maxPerks = 100;
        playerIconIndex = 0;
        highestKillCount = 0;
        sensitivity = 3f;
        musicVolume = 3f;
    }


    public static void LoadDataToGame()
    {
        GameStats.username = username;
        GameStats.currPerks = currPerks;
        GameStats.maxPerks = maxPerks;
        GameStats.playerIconIndex = playerIconIndex;
        GameStats.highestKillCount = highestKillCount;
        GameStats.sensitivity = sensitivity;
        GameStats.musicVolume = musicVolume;
    }
}
