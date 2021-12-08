using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : WaypointTracer
{
    [SerializeField] GameObject fireflyObject;
    
    Firefly firefly { get { return fireflyObject==null ? null : fireflyObject.GetComponent<Firefly>(); } }

    void Start()
    {
        wpSet("w_fruit", (Waypoint w) => w.connectedTo == null);
        wpSet("w_cinderVase", (Waypoint w) => GameObject.Find("GameProgress").GetComponent< GameProgress>().GetGameStage() > 0);
        wpSet("w_treeBottom", (Waypoint w) => w.headDistTo(w.name) < 5);
        wpSet("w_treeTop", (Waypoint w) => w.headDistTo(w.name) < 5);
        wpSet("w_shrine", (Waypoint w) => w.headDistTo(w.name) < 6);
    }

    void Update()
    {
        if(firefly == null)
        {
            return;
        }
        if ( firefly.waypoint == null )
        {
            Debug.Log("Destroying firefly");
            Destroy(fireflyObject);
            return;
        }

        if (firefly.waypoint.testNext(firefly.waypoint) )
        {
            Waypoint prior = firefly.waypoint;
            firefly.waypoint = firefly.waypoint.next == null ? null : firefly.waypoint.next.GetComponent<Waypoint>();
            Debug.Log("Waypoint " + prior.name + " => " + (firefly.waypoint == null ? "END" : firefly.waypoint.name));
        }
    }
}
