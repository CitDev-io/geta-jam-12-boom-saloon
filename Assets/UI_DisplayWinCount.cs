using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_DisplayWinCount : MonoBehaviour
{
    GameTop gameTop;
    TextMeshProUGUI txtElement;

    void Start()
    {
        gameTop = Object.FindObjectOfType<GameTop>();
        txtElement = GetComponent<TextMeshProUGUI>();
        txtElement.text = gameTop.ConsecutiveWins + "";
    }

    void FixedUpdate()
    {
        txtElement.text = gameTop.ConsecutiveWins + "";
    }
}
