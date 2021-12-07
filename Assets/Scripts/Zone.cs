using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public Biome biome
    {
        get
        {
            GameObject p = gameObject;
            Biome b = null;
            while(p != null && b == null)
            {
                p = p.transform.parent.gameObject;
                b = p.GetComponent<Biome>();
            }
            Debug.Assert(b != null);
            return b;
        }
    }

    public GameObject headObject { get { return GameObject.Find("Head"); } }
    public Head head { get { return headObject.GetComponent<Head>(); } }

    void Start()
    {
        
    }

    void Update()
    {
        if( !biome.active )
        {
            biome.ReportDistance(0,0);
            return;
        }
        CapsuleCollider[] capsuleList = GetComponentsInChildren<CapsuleCollider>();
        CapsuleCollider capsule = capsuleList[0];

        float dx = capsule.gameObject.transform.position.x - headObject.transform.position.x;
        float dz = capsule.gameObject.transform.position.z - headObject.transform.position.z;

        float dist2 = dx * dx + dz * dz;
        float radius2 = capsule.radius * capsule.radius;
        if( dist2 < radius2 && (head.closestValidBiome==null || dist2 < head.closestValidBiomeDist2 ) )
        {
            head.closestValidBiome = biome;
            head.closestValidBiomeDist2 = dist2;
            head.closestValidBiomeRadius = capsule.radius;
        }
        if (dist2 < radius2)
        {
            biome.ReportDistance(Mathf.Sqrt(dist2),capsule.radius);
        }
    }
}
