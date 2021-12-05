using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrierManager : Periodic
{
    [SerializeField] GameObject carrierPrefab;

    public override void OnTimerComplete()
    {
        Instantiate(carrierPrefab).Init(10.0f,0.0f);
    }
}
