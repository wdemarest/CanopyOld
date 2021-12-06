using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : Item
{
    [SerializeField] GameObject RainParticles;
    ParticleSystem.EmissionModule RainParticleEmission;

    [SerializeField] float rainHeight = 50;
    [SerializeField] public float fadeDuration = 3;
    [SerializeField] public float rainDuration = 15;
    float timer = 0;

    [SerializeField] AudioSource rainLoop;

    float damageTimer = 0;
    [SerializeField] float damageInterval = 1;
    [SerializeField] float damageAmount = 1;
    float rainIntensity = 0;

    float rainMaxVolume;
    [SerializeField] float rainParticleMaxRate;

    // Start is called before the first frame update
    void Start()
    {
        RainParticleEmission = RainParticles.GetComponent<ParticleSystem>().emission;
        rainMaxVolume = rainLoop.volume;
        timer = 0;
        damageTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        rainLoop.volume = rainIntensity * rainMaxVolume;

        RainParticleEmission.rateOverTime = rainIntensity * rainParticleMaxRate;

        fadeDuration = Mathf.Max(0.01f, fadeDuration);  // prevent division by zero.
        bool isFading = timer > rainDuration - fadeDuration;
        if (!isFading && rainIntensity < 1)
        {
            rainIntensity = Mathf.Min( 1, rainIntensity + (1 / fadeDuration) * Time.deltaTime );
        }

        if(isFading && rainIntensity > 0)
        {
            rainIntensity = Mathf.Max( 0, rainIntensity - (1/fadeDuration) * Time.deltaTime );
            GameObject.Find("EnvironmentManager").GetComponent<EnvironmentManager>().SetOvercast(rainIntensity);
        }

        if ( timer > rainDuration )
        {
            GameObject.Find("EnvironmentManager").GetComponent<EnvironmentManager>().SetOvercast(0);
            Destroy(gameObject);
            return;
        }

        if (!isFading)
        {
            RainParticles.transform.position = headObject.transform.position + new Vector3(0, rainHeight, 0);
            
            bool touchingPlayer = !Physics.Raycast(headObject.transform.position, new Vector3(0, 1, 0), rainHeight, LayerMask.GetMask("GrabbableTerrain"));
            if (touchingPlayer)
            {
                Debug.Log("Rain touching player " + damageTimer);
                damageTimer += Time.deltaTime;
                if (damageTimer >= damageInterval)
                {
                    head.takeDamage(damageAmount);
                    damageTimer = 0;
                }
            }
            else
            {
                damageTimer = 0;
            }
        }
    }
}
