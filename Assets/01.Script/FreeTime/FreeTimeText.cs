using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class FreeTimeText : TextManager
{
    const string URL = "https://docs.google.com/spreadsheets/d/1m5NRdsq4Xlf11jEJZ1dZ3wqUiPYDXl6GGHdL09clN9g/export?format=tsv";

    [SerializeField]
    public GameObject storyPanel;

    [SerializeField]
    private Sprite restaurantOutImage = null;

    private Tutorial_FreeTime _tutorial_FreeTime = null;

    public static bool _istuto = true;

    private InputButton inputButton = null;
    [SerializeField] private GameObject loadingpanel;
    private GameObject _loadingPanel;

    private void Awake()
    {
        StartCoroutine(LoadTextData(URL));
        StartCoroutine(SetDelay1());
    }

    public void Start()
    {
        base.Start();
        StartCoroutine(LoadingPanel());
    }

    IEnumerator LoadingPanel()
    {
        GameObject g = loadingpanel;
        _loadingPanel = Instantiate(g);

        yield return new WaitForSeconds(1.7f);

        _loadingPanel.SetActive(false);
    }

    IEnumerator SetDelay1()
    {
        _tutorial_FreeTime = transform.Find("Tutorial").GetComponent<Tutorial_FreeTime>();
        yield return new WaitForSeconds(0.8f);

        if (_istuto)
        {
            _tutorial_FreeTime.StartTuto();
        }
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

    IEnumerator ActiveButton()
    {
        for (int i = 0; i < 4; i++)
        {
            ButtonManager.Instance.storyButton[i].interactable = true;
            yield return new WaitForSeconds(0.04f);
        }

        _istuto = false;
        GoToMainScreen();
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
            yield return new WaitForSeconds(1.6f);
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
        if (_time < 2)
        {
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
        else
        {
            if(nextCount == 0) FightManager.sendChatID = 8;
            else if(nextCount == 1) FightManager.sendChatID = 10;
            else if(nextCount == 2) FightManager.sendChatID = 12;
            else if(nextCount == 3) FightManager.sendChatID = 7;
            nextCount += 1;
            _time = 0;
            SceneManager.LoadScene("Typing");
        }
    }
}
