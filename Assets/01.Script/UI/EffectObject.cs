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


    void Start()
    {
        TextManager.Instance.OnEffectObject += PlayingEffect;
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

    }

    private void BoolFadeOut()
    {

    }

    private void BoolWalk()
    {

    }
}
