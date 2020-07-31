using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GenericButtonClick : MonoBehaviour
{
    GameTop gameTop;

    void Start()
    {
        gameTop = Object.FindObjectOfType<GameTop>();
    }

    public void GetClicked(){
        gameTop.PlaySound("Click");
    }
}
