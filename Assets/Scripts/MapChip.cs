using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChip : MonoBehaviour
{
    SpriteRenderer sp;
    Animator anim;
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        if (CompareTag("wall"))
        {
            anim.Play("Base Layer.Space",0, Random.value);
        }
    }

    public void ChangeSprite(Sprite _sp)
    {
        sp.sprite = _sp;
    }
    public void LostAnim()
    {
        anim.SetBool("Lost",true);
    }
    public void SetDefault()
    {
        anim.SetBool("Lost", false);
        gameObject.SetActive(true);

    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
