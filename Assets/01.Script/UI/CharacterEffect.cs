using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterEffect : MonoBehaviour
{
    static public bool SkipDotweenAnimation = false; // 닷트윈 스킵 여부 확인 불 값

    public void MoveXposition(float distance, float time, int direct) // 가로로 움직이는 닷트윈 애니메이션
    {
        gameObject.transform.DOMoveX((distance * direct), time).From(distance * direct, SkipDotweenAnimation).OnComplete(() => DoTweenComplete());
    }

    public void DoTweenComplete()
    {
        SkipDotweenAnimation = false;
    }
}
