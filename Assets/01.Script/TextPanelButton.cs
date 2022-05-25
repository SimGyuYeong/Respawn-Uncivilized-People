using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TextPanelButton : MonoBehaviour
{
    public UnityEvent ButtonPress;

    private void OnMouseEnter()
    {
        Debug.Log("Mouse Enter");
    }

    private void OnMouseDown()
    {
        ButtonPress?.Invoke();
    }
}
