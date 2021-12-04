using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeMover : MonoBehaviour
{
    Biome biome;
    float timer = 0;
    float biomeMoveTime = 5;
    AnimationCurve biomeMoveSpeed;
    float biomeTotalMoveDist = 0;
    float yDestination = 0;
    Transform NBT;
    Action callback;

    void Start()
    {

    }

    void EnableParticles(bool state)
    {
        Transform pfxTransform = biome.gameObject.GetComponent<Transform>().Find("BiomeAppearParticles");
        ParticleSystem.EmissionModule emission = pfxTransform.GetComponent<ParticleSystem>().emission;
        emission.enabled = state;
    }

    public void SequenceStart(Biome biome, float biomeMoveTime, AnimationCurve biomeMoveSpeed, Action callback)
    {
        this.biome = biome;
        this.biomeMoveSpeed = biomeMoveSpeed;
        this.biomeMoveTime = biomeMoveTime;
        this.callback = callback;
        NBT = biome.gameObject.transform;
        biomeTotalMoveDist = NBT.position.y- yDestination;
        EnableParticles(true);
    }

    void Update()
    {
        timer = Mathf.Min(biomeMoveTime, timer+Time.deltaTime);
        NBT.position = new Vector3(NBT.position.x, (1 - biomeMoveSpeed.Evaluate(timer / biomeMoveTime)) * biomeTotalMoveDist, NBT.position.z);
        if (timer >= biomeMoveTime)
        {
            SequenceEnd();
            timer = 0;
        }
    }

    void SequenceEnd()
    {
        EnableParticles(false);
        NBT.position = new Vector3(NBT.position.x, yDestination, NBT.position.z);
        callback();
    }
}
