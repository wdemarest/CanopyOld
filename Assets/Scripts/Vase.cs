using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Vase : MonoBehaviour
{
    [SerializeField] GameObject DepositText;
    [SerializeField] int depositTextValue = 0;
    public Renderer renderer;

    int depositsQueued = 0;

    [SerializeField] AudioSource depositSound;

    public bool IsAnimatingScore { get { return depositsQueued > 0; } }

    void Start()
    {
    }

    public void Clear()
    {
        depositTextValue = 0;
        depositsQueued = 0;
    }

    public void NotifyOfDeposit(int points)
    {
        depositsQueued += points;
    }

    public void SetColor(Color color)
    {
        renderer.material.color = color;
    }

    void Update()
    {
        DepositText.GetComponent<TMP_Text>().text = "" + depositTextValue;

        if (!depositSound.isPlaying)
        {
            if(depositsQueued > 0)
            {
                depositTextValue++;
                Debug.Log("playing");
                depositSound.Play();
                depositSound.pitch += 0.2f;
                depositsQueued--;
            }
            else
            {
                depositSound.pitch = 0.75f;
            }
        }
    }
}