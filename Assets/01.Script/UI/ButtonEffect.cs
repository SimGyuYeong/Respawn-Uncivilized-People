using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private float spd = 0.08f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.Find("sprite").DOScaleX(10f, spd);
        transform.Find("Text/Image").DOScaleX(10f, spd);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.Find("sprite").DOScaleX(0f, spd);
        transform.Find("Text/Image").DOScaleX(0f, spd);
    }
}
