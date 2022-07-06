using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

public class InputButton : ButtonManager
{
    private int moveTo = 1;
    public UnityEvent textEvents;

    private int _id;

    private FreeTimeText _freeTimeText = null;

    private string _timetext;

    private void Start()
    {
        _freeTimeText = transform.Find("TextManager").GetComponent<FreeTimeText>();
    }

    public void InputStoryButton(int code)
    {
        _id = code + 5 * FreeTimeText.nextCount;

        if (_id % 4 == 0) { _freeTimeText.StartCoroutine("GoToMain"); }

        StartCoroutine(ButtonMove());
    }

    public void InputSettingButton()
    {
        settingPanel.transform.DOMoveY(0, 0.34f);//.SetEase(Ease.OutBack);
    }

    public void InputContinueButton()
    {
        settingPanel.transform.DOMoveY(11, 0.3f).SetEase(Ease.InCirc);
        //timeText.text = string.Format("{0}st Week Day {1} Post Meridiem", weekCount, dayCount);
    }

    public void ButtonSet()
    {
        StartCoroutine(ButtonSetting());
    }

    IEnumerator ButtonSetting()
    {
        //Sequence seq = DOTween.Sequence();
        for (int i = 0; i < storyButton.Length; i++)
        {
            storyButton[i].transform.DOMoveY(1.24f * moveTo, 0.3f).SetEase(Ease.InOutQuart);
            yield return new WaitForSeconds(0.12f);
        }
    }

    public void ChangeMain()
    {
        if (isDay) 
        {
            timeText.text = string.Format($"{weekCount}st Week Day {dayCount} Ante Meridiem");
            mainImage.sprite = mainBackgroundImage[0];
            isDay = false; 
        }
        else
        {
            timeText.text = string.Format($"{weekCount}st Week Day {dayCount} Post Meridiem");
            mainImage.sprite = mainBackgroundImage[1];
            isDay = true;
            dayCount++;
        }
    }

    IEnumerator WriteTimeText()
    {
        for (int i = 0; i < _timetext.Length; i++)
        {
            timeText.text += _timetext[i];
        }

        yield return null;
    }

    IEnumerator ButtonMove()
    {
        //Sequence seq = DOTween.Sequence();
        for (int i = 0; i < storyButton.Length; i++)
        {
            storyButton[i].transform.DOMoveY(7 * moveTo, 0.3f).SetEase(Ease.InOutQuart);
            yield return new WaitForSeconds(0.12f);
        }

        StartCoroutine(Fade());
    }
    
    IEnumerator Fade()
    {
        Color color = fadeImage.color;

        fadeImage.gameObject.SetActive(true);

        while(fadeImage.color.a <= 1)
        {
            color.a += 0.01f;
            fadeImage.color = color;
            yield return new WaitForSeconds(0.005f);
        }

        storyButton[_id - 1].interactable = false;

        Color textAlpha = storyButton[_id - 1].transform.Find("ButtonImageMask/Image").GetComponent<Image>().color;
        
        textAlpha.a = 170;
        storyButton[_id - 1].transform.Find("ButtonImageMask/Text").GetComponent<TextMeshProUGUI>().color = textAlpha;

        //storyButton[_id - 1].transform.Find("iIntimacy").GetComponent<Button>().interactable = false;

        //textAlpha = storyButton[_id - 1].transform.Find("iIntimacy").Find("IlntimacyText").GetComponent<TextMeshProUGUI>().color;
        //textAlpha.a /= 3;
        //storyButton[_id - 1].transform.Find("iIntimacy").Find("IlntimacyText").GetComponent<TextMeshProUGUI>().color = textAlpha;

        yield return new WaitForSeconds(1.2f);

        _freeTimeText.SetText(_id);


        while (fadeImage.color.a >= 0)
        {
            color.a -= 0.01f;
            fadeImage.color = color;
            yield return new WaitForSeconds(0.005f);
        }
        fadeImage.gameObject.SetActive(false);

        ChangeMain();
    }
}
