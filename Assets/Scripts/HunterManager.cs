using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterManager : Periodic
{
    [SerializeField] GameObject hunterPrefab;

    public override void OnTimerComplete()
    {
        Instantiate(hunterPrefab).Init();
    }
}
