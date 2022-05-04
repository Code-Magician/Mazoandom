using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static string username;
    public static int currLevel = 1;
    public static bool gameOver = false;
    public static int totalZombiesSpawned = 0;
    public static int totalZombiesInCurrentLevel = 0;
    public static int currFloor = 1;
    public static float bulletForce = 400;
    public static float sensitivity = 3f;
    public static bool zombieCanAttack = false;

    public static int currPerks = 0;
    public static int maxPerks = 100;

    public static Sprite playerIcon;
    public static int playerIconIndex = 0;

    public static float musicVolume = 1;

    public static void ReassemblePerks()
    {
        int cnt = 0;
        currPerks += (totalZombiesSpawned - totalZombiesInCurrentLevel);
        while (cnt <= 1000 && currPerks >= maxPerks)
        {
            currPerks -= maxPerks;
            maxPerks += 50;
            cnt++;
        }

        string currperksKey = "currperks" + DB_Manager.username;
        PlayerPrefs.SetInt(currperksKey, currPerks);

        string maxperksKey = "maxperks" + DB_Manager.username;
        PlayerPrefs.SetInt(maxperksKey, maxPerks);

        string sensitivityKey = "sensitivity" + DB_Manager.username;
        PlayerPrefs.SetFloat(sensitivityKey, sensitivity);

        string playerIconIndexKey = "playerIconIndex" + DB_Manager.username;
        PlayerPrefs.SetInt(playerIconIndexKey, DB_Manager.playerIconIndex);
    }

    public static int GetHighScore()
    {
        int highscore = 0;
        int level = (maxPerks - 50) / 50;

        if (level > 1)
            highscore = (level - 2) * (50 * level - 100) + currPerks;
        else
            highscore = currPerks;

        return highscore;
    }

}
