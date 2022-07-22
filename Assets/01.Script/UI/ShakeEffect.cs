using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShakeEffect : MonoBehaviour
{
    public void Shake(float time = 0.13f, float str = 1000)
    {
        transform.DOShakePosition(time, str);
    }
}
