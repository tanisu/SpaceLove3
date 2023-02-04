using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Abutton : MonoBehaviour
{

    [SerializeField] Sprite pushImage;
    Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator _pushA()
    {
        yield return null;
        Sprite defaultSprite = image.sprite;
        image.sprite = pushImage;
        yield return new WaitForSeconds(0.05f);
        image.sprite = defaultSprite;

    }
}
