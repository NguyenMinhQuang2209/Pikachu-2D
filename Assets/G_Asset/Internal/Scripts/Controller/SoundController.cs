using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;
    public AudioSource winSound;
    public AudioSource wrongSound;
    public AudioSource bgSound;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        bgSound.loop = true;
        bgSound.Play();
    }
    public void PlayWinSound()
    {
        winSound.Play();
    }
    public void PlayWrongSound()
    {
        wrongSound.Play();
    }
}
