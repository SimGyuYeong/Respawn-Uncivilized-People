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

    private InputButton inputButton = null;

    private void Awake()
    {
        StartCoroutine(LoadTextData(URL));
    }

    protected override void ChildStart()
    {
        _textPanel = textPanelObj.transform.Find("text").GetComponent<TextMeshPro>();
        inputButton = GetComponentInParent<InputButton>();
        base.ChildStart();
    }

    public void SetText(int _ID)
    {
        chatID = _ID;
        lineNumber = 1;
        textPanelPrefab.SetActive(true);
        textPanelBTN.interactable = true;
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

    public void SetButton()
    {
        inputButton.ButtonSet();
    }

    public void GoToMainScreen()
    {
        storyPanel.transform.position = new Vector3(0, 100, 0);

        //Typing();
    }

    IEnumerator ZoomIn()
    {
        //textPanelBTN.interactable = false;
        FreeTimeDirect.Instance.ZoomIn();
        //SkipText();
        yield return null;
    }

    IEnumerator ZoomOut()
    {
        FreeTimeDirect.Instance.ZoomOut();
        textPanelBTN.interactable = true;
        yield return null;
    }

    IEnumerator TakeALook()
    {
        textPanelBTN.interactable = false;
        FreeTimeDirect.Instance.LookAround(() =>
        {
            SkipText();
            textPanelBTN.interactable = true;
        });

        yield return null;
    }

    IEnumerator Walking()
    {
        //Debug.Log("ascasda");
        textPanelBTN.interactable = false;
        //_textPanel.text = string.Format("");
        //FreeTimeDirect.Instance.FadeIn();
        FreeTimeDirect.Instance.Walking();

        textPanelObj.SetActive(false);
        yield return new WaitForSeconds(1.7f);
        textPanelObj.SetActive(true);
        FreeTimeDirect.Instance.FadeOutTextPanel();
        yield return new WaitForSeconds(0.8f);
        textPanelBTN.interactable = true;
        SkipText();
    }



    IEnumerator FadeOut()
    {
        if (_isEnd == false)
        {
            textPanelBTN.interactable = false;
            FreeTimeDirect.Instance.FadeOut();
            yield return new WaitForSeconds(0.9f);
            textPanelBTN.interactable = true;
            SkipText();
        }

        else
        {
            FreeTimeDirect.Instance.FadeOut();
            yield return new WaitForSeconds(0.9f);
        }
    }

    IEnumerator FadeIn()
    {
        if (_isEnd == false)
        {
            textPanelBTN.interactable = false;
            FreeTimeDirect.Instance.FadeIn();
            yield return new WaitForSeconds(0.9f);
            textPanelBTN.interactable = true;
            SkipText();
        }

        else
        {
            FreeTimeDirect.Instance.FadeIn();
            yield return new WaitForSeconds(0.9f);
        }
    }

    IEnumerator GoToMain()
    {
        _time++;
        _isEnd = true;
        textPanelBTN.interactable = false;
        FreeTimeDirect.Instance.FadeOutTextPanel();
        textPanelObj.gameObject.SetActive(false);
        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(0.9f);
        transform.GetComponentInChildren<FreeTimeText>().GoToMainScreen();
        yield return new WaitForSeconds(0.9f);
        StartCoroutine(FadeIn());
        transform.GetComponentInChildren<FreeTimeText>().SetButton();
    }
}
