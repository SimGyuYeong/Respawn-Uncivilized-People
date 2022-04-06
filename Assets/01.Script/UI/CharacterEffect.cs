using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterEffect : MonoBehaviour
{
    public void MoveXposition(float distance, float time, int direct)
    {
        gameObject.transform.DOMoveX((distance * direct), time);
    }

    public void DoTweenComplete()
    {
        DoTweenComplete();
    }
}
