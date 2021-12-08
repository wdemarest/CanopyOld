using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    Head head {  get { return GameObject.Find("Head").GetComponent<Head>(); } }

    AudioSource fading = null;
    AudioSource music = null;
    AudioSource overlay = null;
    float musicDelay = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMusic();
    }

    void UpdateMusic()
    {
        if (overlay != null && overlay.isPlaying)
        {
            return;
        }
        overlay = null;

        if (fading != null)
        {
            const float fadeDuration = 10;
            fading.volume = Mathf.Max(0, fading.volume - (Time.deltaTime / fadeDuration));
            if (fading.volume <= 0)
            {
                fading = null;
                const float silenceDuration = 10;
                musicDelay = silenceDuration;
            }
        }

        if (fading == null && music != null && !music.isPlaying)
        {
            if (musicDelay > 0)
            {
                musicDelay -= Time.deltaTime;
            }
            if (musicDelay <= 0)
            {
                music.priority = 0;
                music.volume = 1;
                if (!music.isPlaying)
                {
                    music.Play();
                }
                const float musicRepeatDelay = 10;
                musicDelay = musicRepeatDelay;
            }
        }
    }
    public void StopMusic()
    {
        if (fading != null)
        {
            fading.Stop();
            fading = null;
        }
        if (music != null)
        {
            music.Stop();
            music = null;
        }

    }
    public void StopOverlay()
    {
        if (overlay != null)
        {
            overlay.Stop();
            overlay = null;
        }
    }
    public void StopAll()
    {
        StopOverlay();
        StopMusic();
    }

    public void FadeMusic()
    {
        if (fading == null && music != null && music.isPlaying)
        {
            fading = music;
        }
    }


    public void PlayMusic(AudioSource newMusic)
    {
        FadeMusic();
        Debug.Log("Playing music");
        music = newMusic;
        musicDelay = 0;
    }


    public void PlayOverlay(AudioSource newOverlay)
    {
        StopMusic();
        StopOverlay();
        Debug.Log("Playing overlay");
        overlay = newOverlay;
        overlay.volume = 1;
        overlay.Play();
        musicDelay = Mathf.Max(musicDelay,2);
    }
}
