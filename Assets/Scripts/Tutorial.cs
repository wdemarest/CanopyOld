using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : WaypointTracer
{
    [SerializeField] GameObject fireflyObject;

    void Start()
    {
        wpSet("w_treeBottom", (Waypoint w) => w.headDistTo(w.name) < 3);
        wpSet("w_treeTop", (Waypoint w) => w.headDistTo(w.name) < 3);
        wpSet("w_treeEdge", (Waypoint w) => w.headDistTo(w.name) < 2);
        wpSet("w_fruit", (Waypoint w) => w.connectedTo == null);
        wpSet("w_vase", (Waypoint w) => GameObject.Find("CinderBiome") == null);
    }

    void Update()
    {
        if (fireflyObject != null && GameObject.Find("CinderBiome") == null)
        {
            Destroy(fireflyObject);
            return;
        }
        if (fireflyObject != null)
        {
            Firefly firefly = fireflyObject.GetComponent<Firefly>();
            if (firefly.waypoint.testComplete(firefly.waypoint))
            {
                firefly.waypoint = firefly.waypoint.next == null ? null : firefly.waypoint.next.GetComponent<Waypoint>();
            }
        }
    }
}
