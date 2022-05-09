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


     // 캐릭터 이펙트
    public void MoveXposition(float distance, float time, int direct) // 가로로 움직이는 닷트윈 애니메이션
    {
        //gameObject.transform.DOMoveX((distance * direct), time).From(distance * direct, SkipDotweenAnimation).OnComplete(() => DoTweenComplete());
    }
    
    public void DoTweenComplete()
    {
        //SkipDotweenAnimation = false;
    }

    // 화면 이펙트
    public void FadeIn(float time)
    {
        //BackGroundImage.DOFade(1, time).From(SkipDotweenAnimation).OnComplete(() => DoTweenComplete());
    }

    public void Fadeout(float time)
    {
        BackGroundImage.DOFade(0, time).OnComplete(() => BackGroundObject.SetActive(false)); ;
    }
}
