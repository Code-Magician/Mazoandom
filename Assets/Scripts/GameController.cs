using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance
    {
        get;
        set;
    }


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }





}
