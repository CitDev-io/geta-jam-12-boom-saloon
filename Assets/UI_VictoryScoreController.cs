using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_VictoryScoreController : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI clearedBox;
    [SerializeField] public TextMeshProUGUI timeRemainingBox;
    [SerializeField] public TextMeshProUGUI boardMultiplierBox;
    [SerializeField] public TextMeshProUGUI boardTotalBox;
    [SerializeField] public TextMeshProUGUI startingBox;
    [SerializeField] public TextMeshProUGUI gameTotalBox;

    public void UpdateScores(
        int cleared,
        int timeRemaining,
        int boardMultiplier,
        int boardTotal,
        int starting,
        int gameTotal
    ) {
        clearedBox.text = cleared + "";
        timeRemainingBox.text = timeRemaining + "s";
        boardMultiplierBox.text = "x" + boardMultiplier;
        boardTotalBox.text = boardTotal + "";
        startingBox.text = starting + "";
        gameTotalBox.text = gameTotal + "";
    }
}
