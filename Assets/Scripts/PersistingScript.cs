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

    AudioSource backgroundMusic;


    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        backgroundMusic = GetComponent<AudioSource>();
    }


    public void ChangeVolume()
    {
        backgroundMusic.volume = GameStats.musicVolume;
    }

}
