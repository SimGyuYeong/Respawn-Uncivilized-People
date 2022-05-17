using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using TMPro;

public class FreeTimeText : TextManager
{
    const string URL = "https://docs.google.com/spreadsheets/d/1m5NRdsq4Xlf11jEJZ1dZ3wqUiPYDXl6GGHdL09clN9g/export?format=tsv";

    private void Awake()
    {
        StartCoroutine(LoadTextData(URL));
    }

    private void Start()
    {
        _textPanel = textPanelObj.transform.parent.Find("text").GetComponent<TextMeshPro>();
    }

    public void SetText(int _ID)
    {
        chatID = _ID;
        StartCoroutine(TypingCoroutine());
    }
}
