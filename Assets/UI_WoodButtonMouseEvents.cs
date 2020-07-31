using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

 public class UI_WoodButtonMouseEvents : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
 {

     public void OnPointerEnter(PointerEventData eventData)
     {
         GetComponent<Image>().sprite = (Sprite) Resources.Load("Images/Wood2", typeof(Sprite));
     }
 
     public void OnPointerExit(PointerEventData eventData)
     {
         GetComponent<Image>().sprite = (Sprite) Resources.Load("Images/Wood", typeof(Sprite));
     }
 }