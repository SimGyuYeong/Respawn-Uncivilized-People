using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterEffect : MonoBehaviour
{
    static public bool SkipDotweenAnimation = false; // ��Ʈ�� ��ŵ ���� Ȯ�� �� ��

    public void MoveXposition(float distance, float time, int direct) // ���η� �����̴� ��Ʈ�� �ִϸ��̼�
    {
        gameObject.transform.DOMoveX((distance * direct), time).From(distance * direct, SkipDotweenAnimation).OnComplete(() => DoTweenComplete());
    }

    public void DoTweenComplete()
    {
        SkipDotweenAnimation = false;
    }
}
