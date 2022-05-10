using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EffectObject : MonoBehaviour
{
    [Header("실행하고 싶은 배경 이미지 이벤트")]
    [SerializeField]
    private bool boolFadeIn = false;
    [SerializeField]
    private bool boolFadeOut = false;
    [SerializeField]
    private bool boolWalk = false;

    //[Header("실행하고 싶은 ")]

    private SpriteRenderer spriteRenderer;

    static public bool SkipDotweenAnimation = false;

    void Start()
    {
        TextManager.Instance.OnEffectObject += PlayingEffect;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayingEffect(GameObject g)
    {
        if(g == gameObject)
        {
            if(boolFadeIn == true)
            {
                BoolFadeIn();
            }
            if (boolFadeOut == true)
            {
                BoolFadeOut();
            }
            if (boolWalk == true)
            {
                BoolWalk();
            }
        }
    }

    private void BoolFadeIn()
    {
        spriteRenderer.DOFade(1, 0.5f).From(SkipDotweenAnimation).OnComplete(() => SkipDotweenAnimation = false);
    }

    private void BoolFadeOut()
    {
        spriteRenderer.DOFade(0, 0.5f).From(SkipDotweenAnimation).OnComplete(() => SkipDotweenAnimation = false);
    }

    private void BoolWalk()
    {
        //TextManager.Instance.effectObject.transform.DOMoveY(0.5f, 0.3f);
        //TextManager.Instance.effectObject.transform.DOMove(Vector2.zero, 0.3f);
        //TextManager.Instance.effectObject.transform.DOMoveY(0.4f, 0.3f);
        //TextManager.Instance.effectObject.transform.DOMove(Vector2.zero, 0.3f);
        //TextManager.Instance.effectObject.transform.DOMoveY(0.3f, 0.3f);
        //TextManager.Instance.effectObject.transform.DOMove(Vector2.zero, 0.3f);
        //TextManager.Instance.effectObject.transform.DOMoveY(0.2f, 0.3f);
        //TextManager.Instance.effectObject.transform.DOMove(Vector2.zero, 0.3f);
    }
}
