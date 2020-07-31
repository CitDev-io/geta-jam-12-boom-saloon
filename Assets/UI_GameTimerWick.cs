using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameTimerWick : MonoBehaviour
{
    RectTransform WickImage;
    GameTop gameTop;
    PuzzleManager puzzleManager;
    [SerializeField] public GameObject spark;
    [SerializeField] public GameObject burntWick;

    float maxWidth = 800f;

    void Start()
    {
        WickImage = GetComponent<RectTransform>();
        gameTop = Object.FindObjectOfType<GameTop>();
        puzzleManager = Object.FindObjectOfType<PuzzleManager>();
    }

    void FixedUpdate()
    {
        float timePercent = 1 - (puzzleManager.timeLeft / gameTop.SecondsPerBoard);
        if (spark != null) {
            spark.GetComponent<RectTransform>().localPosition = new Vector3((timePercent * maxWidth) - 780f, 0f, 0f);
        }
        burntWick.GetComponent<RectTransform>().sizeDelta = new Vector2(timePercent * maxWidth, 15f);
    }
}
