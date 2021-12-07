using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointTracer : MonoBehaviour
{
    public Waypoint wp(string name)
    {
        return GameObject.Find(name).GetComponent<Waypoint>();
    }
    public void wpSet(string name, Func<Waypoint, bool> testComplete)
    {
        wp(name).testComplete = testComplete;
    }
}

public class Waypoint : MonoBehaviour
{
    public GameObject connectedTo;
    public GameObject next;
    public Func<Waypoint, bool> testComplete;

    public GameObject headObject { get { return GameObject.Find("Head"); } }
    public Vector3 headPos { get { return headObject.GetComponent<Transform>().position; } }
    public GameObject parent { get { Transform t = gameObject.GetComponent<Transform>(); return t.parent==null ? null : t.parent.gameObject; } }

    public float headDistTo(string gameObjName)
    {
        GameObject g = GameObject.Find(gameObjName);
        return Vector3.Distance(g.GetComponent<Transform>().position, headPos);
    }

    void Start()
    {
        
    }

    void Update()
    {
                
    }
}
