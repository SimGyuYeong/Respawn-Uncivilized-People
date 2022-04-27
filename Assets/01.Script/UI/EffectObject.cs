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
