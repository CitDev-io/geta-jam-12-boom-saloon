using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UI_BombCounter : MonoBehaviour
{
    PuzzleManager puzzleManager;
    [SerializeField] GameObject bomb1;
    [SerializeField] GameObject bomb2;
    [SerializeField] GameObject bomb3;

    void Start() {
        puzzleManager = UnityEngine.Object.FindObjectOfType<PuzzleManager>();
    }

    void FixedUpdate() {
        try {
            if (puzzleManager.ReadyToChoose() && puzzleManager.timeLeft > 0f) {
                int bombs = puzzleManager.GetCurrentBombCount();
                bomb1.SetActive(bombs > 0);
                bomb2.SetActive(bombs > 1);
                bomb3.SetActive(bombs > 2);
            }
        } catch (Exception e) {

        }
    }
}
