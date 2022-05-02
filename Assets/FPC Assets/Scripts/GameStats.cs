using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    public static int currLevel = 1;
    public static bool gameOver = false;
    public static int totalZombiesInCurrentLevel = 0;
    public static int currFloor = 1;
    public static float bulletForce = 400;
    public static float sensitivity = 3f;
    public static bool zombieCanAttack = false;
}
