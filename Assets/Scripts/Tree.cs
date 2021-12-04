using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = new Quaternion(0, Random.value * Mathf.PI, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
