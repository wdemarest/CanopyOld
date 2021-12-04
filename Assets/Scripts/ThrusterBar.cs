using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterBar : MonoBehaviour
{
    Head HeadScript;
    [SerializeField] GameObject Bar;
    
    // Start is called before the first frame update
    void Start()
    {
        HeadScript = GameObject.Find("Head").GetComponent<Head>();
    }

    // Update is called once per frame
    void Update()
    {
        float thrustPercentRemaining = HeadScript.thrustRemaining / HeadScript.thrustMax;

        Bar.transform.localPosition = new Vector3(0.06f, 0, (1-thrustPercentRemaining)*-1.5f);
        Bar.transform.localScale = new Vector3(0, 1, thrustPercentRemaining * 3);
    }
}
