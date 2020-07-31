using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_PauseButtonClick : MonoBehaviour, IPointerClickHandler
{ 
    PuzzleManager puzzleManager;

    void Start() {
        puzzleManager = Object.FindObjectOfType<PuzzleManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        puzzleManager.RequestPause();
    }
}
