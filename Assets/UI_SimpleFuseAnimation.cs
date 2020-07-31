using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SimpleFuseAnimation : MonoBehaviour
{
    [SerializeField] Sprite sprite1;
    [SerializeField] Sprite sprite2;
    Image image;
    PuzzleManager puzzleManager;

    void Start()
    {
        image = GetComponent<Image>();
        puzzleManager = Object.FindObjectOfType<PuzzleManager>();
        StartCoroutine(AnimateFuse());
    }

    IEnumerator AnimateFuse()
    {
        while(!puzzleManager.GameOver) {
            image.sprite = sprite1;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(-18, 18) * 1.1f));
            yield return new WaitForSeconds(Random.Range(1, 4) * 0.03f);
            image.sprite = sprite2;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(-18, 18) * 1.1f));
            yield return new WaitForSeconds(Random.Range(1, 4) * 0.03f);
        }

        Destroy(gameObject);
    }
}
