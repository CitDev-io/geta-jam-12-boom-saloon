using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TITLE_tumbleweedLoop : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 3f;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] GameObject PickUpSpot;
    [SerializeField] GameObject DropSpot;

    void Start()
    {

    }

    void FixedUpdate() {
        transform.position = transform.position + new Vector3(-1f * (moveSpeed / 1000f), 0f, 0f);
        transform.Rotate(new Vector3(0f, 0f, transform.rotation.z + rotationSpeed));


        if (transform.position.x < PickUpSpot.transform.position.x) {
            transform.position = new Vector3(DropSpot.transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
