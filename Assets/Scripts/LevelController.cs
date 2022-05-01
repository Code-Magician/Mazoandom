using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelController : MonoBehaviour
{
    [Header("Maximum Level Attributes")]
    [SerializeField] int maxwidth = 20;
    [SerializeField] int maxdepth = 20;
    [SerializeField] int maxFloors = 10;


    [Header("Current Level Description")]
    [SerializeField] int level = 1;
    [SerializeField] int width = 10;
    [SerializeField] int depth = 10;
    [SerializeField] int floors = 3;
    [SerializeField] MultiDungeonManager multiDungeonManager;


    [Header("UI References")]
    [SerializeField] TMP_Text currFloorText;
    [SerializeField] TMP_Text goalFloorText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text zombiesLeftText;



    private void Awake()
    {
        BuildLevel();
    }


    public void BuildLevel()
    {
        Reset();

        goalFloorText.text = "Goal Floor : " + floors;

        multiDungeonManager.totalLevels = Mathf.Min(floors, maxFloors);
        multiDungeonManager.width = Mathf.Min(width, maxwidth);
        multiDungeonManager.depth = Mathf.Min(depth, maxdepth);
        multiDungeonManager.Build();

        zombiesLeftText.text = "Zombies Left : " + GameStats.totalZombiesInCurrentLevel;
    }

    private void Reset()
    {
        for (int i = 0; i < multiDungeonManager.transform.childCount; i++)
        {
            Destroy(multiDungeonManager.transform.GetChild(i).gameObject);
        }
        GameStats.totalZombiesInCurrentLevel = 0;
        GameStats.gameOver = false;
        GameStats.currLevel = 1;

        CancelInvoke("RefreshCurrFloor");
        InvokeRepeating("RefreshCurrFloor", 0, 2f);
    }

    public void MoveToNextLevel()
    {
        level++;
        levelText.text = "Level : " + level;

        int randINT = Random.Range(0, 90);
        if (randINT < 30)
        {
            width = Mathf.Clamp(width + 1, 10, maxwidth);
        }
        else if (randINT > 60)
        {
            depth = Mathf.Clamp(depth + 1, 10, maxdepth);
        }
        else
        {
            floors = Mathf.Clamp(floors + 1, 3, maxFloors);
        }


        BuildLevel();
    }

    private void RefreshCurrFloor()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position, -Vector3.up);
        if (Physics.Raycast(ray, out hit))
        {
            GameObject obj = hit.collider.gameObject;
            if (obj != null)
            {
                Maze m = obj.GetComponentInParent<Maze>();
                if (m != null)
                {
                    currFloorText.text = "Current Floor : " + (m.level + 1);
                    GameStats.currLevel = m.level + 1;
                }
            }
        }
    }
}
