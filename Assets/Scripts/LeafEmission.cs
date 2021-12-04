using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafEmission : MonoBehaviour
{
    [SerializeField] ParticleSystem LeafParticles1;
    [SerializeField] ParticleSystem LeafParticles2;
    GameObject Head;

    // Start is called before the first frame update
    void Start()
    {
        Head = GameObject.Find("Head");
    }

    // Update is called once per frame
    void Update()
    {
        float leafEmission = 1 - ((Head.GetComponent<Transform>().position.y - 40) / 130);

        var em = LeafParticles1.emission;
        em.rateOverTime = leafEmission * 8;

        em = LeafParticles2.emission;
        em.rateOverTime = leafEmission * 4;
    }
}
