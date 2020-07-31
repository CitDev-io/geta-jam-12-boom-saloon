using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using gamejam;
using UnityEngine.UI;

public class LastPlayedUI : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI lptitle;
    [SerializeField] public TextMeshProUGUI lpname;
    [SerializeField] public TextMeshProUGUI lpdescription;
    [SerializeField] public Image icon;

    public void SetTitle(string text) {
        lptitle.text = text;
    }

    public void SetDescription(string text) {
        lpdescription.text = text;
    }

    public void SetName(string text, CardType type) {
        lpname.text = text;

        if (type == CardType.BAD) {
            lpname.color = new Color(0.461f, 0.141f, 0.113f, 1f);
        }
        if (type == CardType.GOOD) {
            lpname.color = new Color(0.113f, 0.461f, 0.176f, 1f);
        }
        if (type == CardType.NEUTRAL) {
            lpname.color = Color.black;
        }

    }

    public void SetIcon(CardFace face) {
        icon.sprite = (Sprite) Resources.Load("Images/" + face.ToString() + "-SPRITE", typeof(Sprite));
    }
}
