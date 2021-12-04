using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    public AudioSource music;

    [SerializeField] float timeToMusic;
    
    // Start is called before the first frame update
    void Start()
    {
        timeToMusic = 15;
    }

    // Update is called once per frame
    void Update()
    {
        timeToMusic -= Time.deltaTime;
        if(timeToMusic <= 0)
        {
            SelectAmbientMusic();
            timeToMusic = 300;
        }
    }

    void SelectAmbientMusic()
    {
        music.Play();
    }
}
