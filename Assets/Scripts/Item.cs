using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float vibDur = 0.5f;
    public int points = 0;
    public bool allowCollectWhenBasketFull = true;
    public string myBiomeName = "";
    public int myMarkerIndex = -1;

    public virtual void OnHandCollide()
    {
        Debug.Log("itemHandCol");
    }

    public virtual void OnDeath()
    {
        bool OK = GameObject.Find("GameProgress").GetComponent<GameProgress>().OnItemDeath(this);
        Destroy(gameObject);
    }
}
