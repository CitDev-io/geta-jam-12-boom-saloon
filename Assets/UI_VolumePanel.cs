using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_VolumePanel : MonoBehaviour, IPointerClickHandler
{ 
    GameTop gameTop;
    bool showPanel = false;
    [SerializeField] public GameObject volumeSelectionPanel;

    void Start() {
        gameTop = Object.FindObjectOfType<GameTop>();
        SetIconToVolumeLevel(gameTop.currentVolume);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TogglePanel();
    }

    void TogglePanel() {
        gameTop.PlaySound("Click");
        showPanel = !showPanel;
        volumeSelectionPanel.SetActive(showPanel);
    }

    public void ChooseVolumeLevel(int volume) {
        gameTop.SetSoundLevel(volume);
        SetIconToVolumeLevel(volume);
        if (showPanel) { TogglePanel(); }
    }

    void SetIconToVolumeLevel(int volume) {
        GetComponent<Image>().sprite = (Sprite) Resources.Load("Icons/volume" + volume, typeof(Sprite));
    }
}
