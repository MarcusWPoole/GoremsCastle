using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{

    [SerializeField] public GameControl gameController;
    [SerializeField] public GameOver gameOverScript;

    [Header("Music")]
    [SerializeField] public AudioClip tutorialMusic;
    [SerializeField] public AudioClip waveMusic;
    [SerializeField] public AudioClip prepMusic;

    private AudioSource musicPlayer;

    private bool waveMusicPlaying;
    private bool prepMusicPlaying;
    private bool tutorialMusicPlaying;

    private float volume;

    // Start is called before the first frame update
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();

        waveMusicPlaying = prepMusicPlaying = tutorialMusicPlaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOverScript.gameEnding)
        {
            StopAllMusic();
            return;
        }
        else
        {
            if (gameController.tutorialController.tutorialRunning)
            {
                PlayTutorialMusic();
            }
            else if (gameController.prepController.prepInProgress)
            {
                PlayTutorialMusic();
            }
            else if (gameController.waveController.waveInProgress)
            {
                PlayWaveMusic();
            }
        }

    }

    void PlayWaveMusic()
    {
        if (musicPlayer.isPlaying)
        {
            if (musicPlayer.clip == waveMusic)
            {
                return;
            }
            else
            {
                StartCoroutine(FadeOutMusic());
                musicPlayer.clip = waveMusic;
                musicPlayer.Play();
                waveMusicPlaying = true;
            }
        }
        else
        {
            musicPlayer.clip = waveMusic;
            musicPlayer.Play();
            waveMusicPlaying = true;
        }
    }

    void PlayPrepMusic()
    {
        if (musicPlayer.isPlaying)
        {
            if (musicPlayer.clip == prepMusic)
            {
                return;
            }
            else
            {
                StartCoroutine(FadeOutMusic());
                musicPlayer.clip = prepMusic;
                musicPlayer.Play();
                prepMusicPlaying = true;
            }
        }
        else
        {
            musicPlayer.clip = prepMusic;
            musicPlayer.Play();
            prepMusicPlaying = true;
        }
    }
    
    void PlayTutorialMusic()
    {
        if (musicPlayer.isPlaying)
        {
            if (musicPlayer.clip == tutorialMusic)
            {
                return;
            }
            else
            {
                StartCoroutine(FadeOutMusic());
                musicPlayer.clip = tutorialMusic;
                musicPlayer.Play();
                tutorialMusicPlaying = true;
            }
        }
        else
        {
            musicPlayer.clip = tutorialMusic;
            musicPlayer.Play();
            tutorialMusicPlaying = true;
        }
    }

    void StopAllMusic()
    {
        musicPlayer.Stop();
    }

    IEnumerator FadeOutMusic()
    {
        volume = musicPlayer.volume;

        while (musicPlayer.volume > 0)
        {
            musicPlayer.volume -= volume * Time.deltaTime / 0.1f;
            yield return null;
        }

        musicPlayer.Stop();
        musicPlayer.volume = volume;
    }

}
