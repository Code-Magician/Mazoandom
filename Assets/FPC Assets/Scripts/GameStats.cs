using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static int currLevel = 1;
    public static bool gameOver = false;
    public static int totalZombiesSpawned = 0;
    public static int totalZombiesInCurrentLevel = 0;
    public static int currFloor = 1;
    public static float bulletForce = 400;
    public static bool zombieCanAttack = false;
    public static Sprite playerIcon;


    // Database me store honge...
    public static float sensitivity = 3f;
    public static int currPerks = 0;
    public static int maxPerks = 100;

    public static int playerIconIndex = 0;

    public static string username;
    public static int highestKillCount = 0;
    public static float musicVolume = 1;



    public static void SaveData()
    {
        int cnt = 0;
        currPerks += (totalZombiesSpawned - totalZombiesInCurrentLevel);
        while (cnt <= 1000 && currPerks >= maxPerks)
        {
            currPerks -= maxPerks;
            maxPerks += 50;
            cnt++;
        }

    }

    public static int GetHighScore()
    {
        return highestKillCount;
    }





}
