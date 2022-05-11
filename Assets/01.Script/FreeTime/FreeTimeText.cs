using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using TMPro;

public class FreeTimeText : TextManager
{
    const string TextURL = "https://docs.google.com/spreadsheets/d/1Cj-vgW7JDlAjLLz4-zX9PXNoJlbsVDCPbt84g8C_It8/edit#gid=94893557";

    [SerializeField] private TextMeshPro text;
    public GameObject textPanel;

    Dictionary<int, string[,]> Sentence = new Dictionary<int, string[,]>();

    private void Awake()
    {
        StartCoroutine(LoadTextData(TextURL, Sentence));
    }

    public void SetText(int _ID)
    {
        chatID = _ID;
        StartCoroutine(TypingCoroutine(text));
    }
}
