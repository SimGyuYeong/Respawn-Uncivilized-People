using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SettingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private TextMeshProUGUI text;
    public UnityEvent ButtonDown;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(text.color != TextUIManager.instance.selectColor)
        {
            text.color = Color.white;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(text.color != TextUIManager.instance.selectColor)
        {
            text.color = Color.gray;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ButtonDown.Invoke();
    }
}
