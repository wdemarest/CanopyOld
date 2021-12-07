using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firefly : Item
{
    public GameObject startingWaypoint;
    public Waypoint waypoint;

    Vector3 position {
        get { return gameObject.GetComponent<Transform>().position; }
        set { gameObject.GetComponent<Transform>().position = value; }
    }
    Vector3 center;
    public float distanceToStayFromPlayer = 3.0f;
    public float timeToRecenter = 3;
    public float yOffset = 2;
    public float speed = 1.5f;
    float hAngle = 0;
    public float hAngleRate = 0.2f;
    float vAngle = 0;
    public float vAngleRate = 0.2f;
    public float radius = 1;

    void Start()
    {
        waypoint = startingWaypoint.GetComponent<Waypoint>();
    }

    void Update()
    {
        if(waypoint == null)
        {
            return;
        }
        Vector3 waypointPos = waypoint.gameObject.GetComponent<Transform>().position;
        Vector3 wayToPlayer = headObject.GetComponent<Transform>().position - waypointPos;

        float dist = wayToPlayer.magnitude - distanceToStayFromPlayer; // Mathf.Max(wayToPlayer.magnitude - distanceToStayFromPlayer, wayToPlayer.magnitude / 2);
        Vector3 temp = dist <= 0 ? waypointPos : waypointPos + wayToPlayer.normalized * dist;
        Vector3 newCenter = new Vector3(temp.x, (temp.y + headObject.GetComponent<Transform>().position.y + yOffset) / 2f, temp.z);

        float distToCenter = Vector3.Distance(center, newCenter);
        center = Vector3.MoveTowards(center, newCenter, (distToCenter/ timeToRecenter) * Time.deltaTime); 

        if( dist < 0 )
        {
            radius = Mathf.Max(0.1f, radius - Time.deltaTime/2);
        }
        else
        {
            radius = Mathf.Min(1.0f, radius + Time.deltaTime/2);
        }

        hAngle += hAngleRate * speed * Time.deltaTime;
        vAngle += vAngleRate * speed * Time.deltaTime;
        Vector3 offset = new Vector3(
            Mathf.Cos(hAngle) * radius,
            Mathf.Sin(vAngle) * radius,
            Mathf.Sin(hAngle+vAngle) * radius
        );

        if (UnityEngine.Random.Range(0, 60 / Time.deltaTime) < 30)
        {
            float m = UnityEngine.Random.Range(0, 100) < 50 ? 1.3f : (1f / 1.3f);
            if (UnityEngine.Random.Range(0, 100) < 50)
            {
                hAngleRate = -hAngleRate*m;
            }
            else
            {
                vAngleRate = -vAngleRate*m;
            }
        }
        hAngleRate += UnityEngine.Random.Range(-0.2f/4f, 0.2f/4f) * Time.deltaTime;
        vAngleRate += UnityEngine.Random.Range(-0.2f/4f, 0.2f/4f) * Time.deltaTime;

        position = center + offset;
    }
}
