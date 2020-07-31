using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTPersister : MonoBehaviour
{
    [SerializeField] public GameObject GameTop;

    void Awake()
    {
        if (GameObject.Find("DDOL_GameTop(Clone)") == null) {
            GameObject.Instantiate(GameTop);
        }
    }
}
