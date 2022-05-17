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

    [SerializeField]
    public GameObject storyPanel;


    private void Awake()
    {
        StartCoroutine(LoadTextData(URL));
    }

    private void Start()
    {
        _textPanel = textPanelObj.transform.Find("text").GetComponent<TextMeshPro>();
    }

    public void SetText(int _ID)
    {
        chatID = _ID;
        TextTyping?.Invoke();
    }

    public override void SetBackground()
    {
        storyPanel.transform.position = new Vector3(0, 0, 0);

        SpriteRenderer _backgroundImage = storyPanel.transform.Find("BackGroundImage").GetComponent<SpriteRenderer>();

        int _backgroundId;
        bool result = Int32.TryParse(Sentence[chatID][lineNumber, (int)IDType.BackgroundID].Trim(), out _backgroundId);
        Debug.Log("a" + _backgroundId + "a");
        if (result) _backgroundImage.sprite = ButtonManager.Instance.gameScreenBackground[_backgroundId];
        else return;
    }

    public void GoToMainScreen()
    {
        storyPanel.transform.position = new Vector3(0, 100, 0);

        Typing();
    }
}
