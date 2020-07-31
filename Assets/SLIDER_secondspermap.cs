using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SLIDER_secondspermap : MonoBehaviour
{
    GameTop gametop;
    [SerializeField] public Text spmText;

    void Start()
    {
        gametop = Object.FindObjectOfType<GameTop>();
        SetSecondsPerBoard(25);
    }

    public void SetSecondsPerBoard(float sec) {
        gametop.SecondsPerBoard = sec;
        spmText.text = sec + "";
    }
}
