using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InputButton : ButtonManager
{
    private int moveTo = 1;

    public void InputStoryButton(int code)  //건물 버튼 누르면 생기는 일
    {
        StartCoroutine(ButtonMove());       //버튼들이 올라감
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

        StartCoroutine(PopUp());
        yield return new WaitForSeconds(1.4f);

        while (fadeImage.color.a >= 0)
        {
            color.a -= 0.01f;
            fadeImage.color = color;
            yield return new WaitForSeconds(0.005f);
        }
    }


    IEnumerator PopUp()
    {
        GameScreen.transform.position = Vector3.zero;
        yield return new WaitForSeconds(1.1f);
    }
}
