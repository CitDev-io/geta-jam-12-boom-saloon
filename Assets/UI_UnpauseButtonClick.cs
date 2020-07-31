using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_UnpauseButtonClick : MonoBehaviour
{
    PuzzleManager puzzleManager;

    void Start()
    {
        puzzleManager = Object.FindObjectOfType<PuzzleManager>();
    }

    public void Unpause() {
        puzzleManager.RequestUnpause();
    }
}
