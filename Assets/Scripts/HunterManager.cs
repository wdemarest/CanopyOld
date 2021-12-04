using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterManager : Periodic
{
    [SerializeField] GameObject hunterPrefab;

    public override void OnTimerComplete()
    {
        Vector3 position = GameObject.Find("Head").GetComponent<Transform>().position;
        position.x -= 50.0f;
        Instantiate(hunterPrefab, position, Quaternion.Euler(0, 0, 0));
    }
}
