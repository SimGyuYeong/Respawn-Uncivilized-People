using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShakingButton : MonoBehaviour
{
    public void ShakeButton()
    {
        Camera.main.GetComponent<CameraShaking>().ShakeForTime(0.2f);
    }
}
