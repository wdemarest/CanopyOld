using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidCloudManager : Periodic
{
    [SerializeField] GameObject acidCloudPrefab;

    public override void OnTimerComplete()
    {
        Vector3 position = GameObject.Find("Head").GetComponent<Transform>().position;
        float zDestroyOffset = 100.0f;
        position.z -= zDestroyOffset;
        GameObject acidCloudObject = Instantiate(acidCloudPrefab, position, Quaternion.Euler(0, 0, 0));
    }
}
