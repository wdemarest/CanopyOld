using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    [SerializeField] public bool Fruit = true;
    [SerializeField] public bool Mine = false;
    [SerializeField] public bool Turret = false;
    [SerializeField] public bool Chaser = false;
    [SerializeField] public bool Floating = false;

    public string typeMade = "";

    public Dictionary<string, bool> flags =
        new Dictionary<string, bool>();

    // Start is called before the first frame update
    void Start()
    {
        flags.Add("Fruit", Fruit);
        flags.Add("Mine", Mine);
        flags.Add("Turret", Turret);
        flags.Add("Chaser", Chaser);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
