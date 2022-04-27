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


    void Start()
    {
        TextManager.Instance.OnEffectObject += PlayingEffect;
    }

    private void PlayingEffect(GameObject g)
    {
        if(g == this.gameObject)
        {

        }
    }
}
