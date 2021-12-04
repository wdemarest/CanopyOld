/*
 * using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceStep
{
    Sequence sequence = null;
    bool running = false;
    public virtual void SequenceStart()
    {
    }

    public virtual void SequenceUpdate()
    {
    }

    void OnDestroy()
    {
        sequence.Advance();
    }
}


public class Sequence : MonoBehaviour
{
    public int index = -1;
    List<Func<SequenceStep>> stepList = new List<Func<SequenceStep>>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Add(Action action)
    {
        step.sequence = this;
        stepList.Add(step);
    }

    void Advance()
    {
        index += 1;
    }

    // Update is called once per frame
    void Update()
    {
        if( index >= stepList.Count )
        {
            return;
        }
        if( !stepList[index].running )
        {
            stepList[index].SequenceStart();
            stepList[index].running = true;
        }
        stepList[index].SequenceUpdate();        
    }
}
*/