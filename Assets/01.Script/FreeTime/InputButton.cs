using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class InputButton : ButtonManager
{
    private int moveTo = 1;
    public UnityEvent textEvents;

    private int _id;

    private FreeTimeText _freeTimeText = null;

    private void Start()
    {
        _freeTimeText = transform.Find("TextManager").GetComponent<FreeTimeText>();
    }

    public void InputStoryButton(int code)
    {
        _id = code;
        //Debug.Log(weekCount.ToString() + dayCount.ToString());
        //timeText.text = string.Format($"{weekCount}st Week Day {dayCount} Post Meridiem");
        StartCoroutine(ButtonMove());
    }

    public void InputSettingButton()
    {
        settingPanel.transform.DOMoveY(0, 0.3f).SetEase(Ease.InCirc);
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
            storyButton[i].transform.DOMoveY(0.8f * moveTo, 0.3f).SetEase(Ease.InOutQuart);
            yield return new WaitForSeconds(0.12f);
        }
    }

    public void ChangeMain()
    {
        if (isDay) 
        {
            timeText.SetText($"{weekCount}st Week Day {dayCount} Ante Meridiem");
            mainImage.sprite = mainBackgroundImage[0];
            isDay = false; 
        }
        else 
        {
            timeText.SetText($"{weekCount}st Week Day {dayCount} Post Meridiem");
            mainImage.sprite = mainBackgroundImage[1];
            isDay = true;
            dayCount++;
        }
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
