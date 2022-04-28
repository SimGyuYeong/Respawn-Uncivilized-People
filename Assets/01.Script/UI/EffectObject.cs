using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EffectObject : MonoBehaviour
{
    [Header("�����ϰ� ���� ��� �̹��� �̺�Ʈ")]
    [SerializeField]
    private bool boolFadeIn = false;
    [SerializeField]
    private bool boolFadeOut = false;
    [SerializeField]
    private bool boolWalk = false;

    //[Header("�����ϰ� ���� ")]

    private SpriteRenderer spriteRenderer;

    static public bool SkipDotweenAnimation = false;

    void Start()
    {
        TextManager.Instance.OnEffectObject += PlayingEffect;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void PlayingEffect(GameObject g)
    {
        if(g == this.gameObject)
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

    }

    private void BoolWalk()
    {

    }
}
