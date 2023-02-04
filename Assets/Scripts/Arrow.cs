using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] Sprite pushSprite;
    SpriteRenderer sp;
    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }

    public IEnumerator ChangeSprite()
    {
        Sprite defaultSprite = sp.sprite;
        yield return null;
        sp.sprite = pushSprite;
        yield return new WaitForSeconds(0.05f);
        sp.sprite = defaultSprite;
    }

}
