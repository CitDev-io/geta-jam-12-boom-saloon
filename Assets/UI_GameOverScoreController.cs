using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_GameOverScoreController : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI clearedBox;
    [SerializeField] public TextMeshProUGUI gameTotalBox;

    public void UpdateScores(
        int cleared,
        int gameTotal
    ) {
        clearedBox.text = cleared + "";
        gameTotalBox.text = gameTotal + "";
    }
}
