using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Vase : MonoBehaviour
{
    [SerializeField] GameObject DepositText;
    [SerializeField] int depositTextValue = 0;
    public Renderer renderer;

    [SerializeField] public int depositedScore = 0;
    int depositsQueued = 0;

    [SerializeField] AudioSource depositSound;

    public bool IsAnimatingScore { get { return depositsQueued > 0; } }

    void Start()
    {
        Clear();
    }
    public void Deposit(int points)
    {
        GameObject.Find("GameProgress").GetComponent<GameProgress>().vaseLastTouched = this;
        depositsQueued += points;
        depositedScore += points;
    }

    public void Clear()
    {
        depositTextValue = 0;
        depositsQueued = 0;
        depositedScore = 0;
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