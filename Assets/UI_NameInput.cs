using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_NameInput : MonoBehaviour
{
    GameTop gameTop;

    void Start()
    {
        gameTop = Object.FindObjectOfType<GameTop>();
        if (gameTop.PlayerName != "Mystery Rustler") {
            GetComponent<TMP_InputField>().text = gameTop.PlayerName;
        }
    }

    public void UpdateUserName(string name) {
        gameTop.PlayerName = name;
    }
}
