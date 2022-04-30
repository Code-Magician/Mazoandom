using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistingScript : MonoBehaviour
{
    public static PersistingScript Instance
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
