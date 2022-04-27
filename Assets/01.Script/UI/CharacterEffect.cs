using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CharacterEffect : MonoBehaviour
{
    [SerializeField]
    private Image BackGroundImage = null;
    [SerializeField]
    private GameObject BackGroundObject = null;

    static public bool SkipDotweenAnimation = false; // ��Ʈ�� ��ŵ ���� Ȯ�� �� ��

     // ĳ���� ����Ʈ
    public void MoveXposition(float distance, float time, int direct) // ���η� �����̴� ��Ʈ�� �ִϸ��̼�
    {
        gameObject.transform.DOMoveX((distance * direct), time).From(distance * direct, SkipDotweenAnimation).OnComplete(() => DoTweenComplete());
    }
    
    public void DoTweenComplete()
    {
        SkipDotweenAnimation = false;
    }

    // ȭ�� ����Ʈ
    public void FadeIn(float time)
    {
        BackGroundImage.DOFade(1, time).From(SkipDotweenAnimation).OnComplete(() => DoTweenComplete());
    }

    public void Fadeout(float time)
    {
        BackGroundImage.DOFade(0, time).OnComplete(() => BackGroundObject.SetActive(false)); ;
    }
}
