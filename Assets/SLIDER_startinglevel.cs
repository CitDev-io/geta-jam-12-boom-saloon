using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SLIDER_startinglevel : MonoBehaviour
{
    GameTop gametop;
    [SerializeField] public Text slText;

    void Start()
    {
        gametop = Object.FindObjectOfType<GameTop>();
        SetStartingLevel(3);
    }

    public void SetStartingLevel(float lvl) {
        gametop.BoardSize = (int) lvl;
        slText.text = lvl + "";
    }
}
