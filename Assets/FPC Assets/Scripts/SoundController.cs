using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioSource fireAudio;
    [SerializeField] FPController FPC;


    public void Fire()
    {
        if (FPC.canShoot)
            fireAudio.Play();
    }


    public void Reload()
    {
        FPC.Reload();
    }
}
