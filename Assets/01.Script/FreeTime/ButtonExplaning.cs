using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonExplaning : ButtonManager, IPointerExitHandler, IPointerEnterHandler
{
    public TextMeshProUGUI explaningText;
    public Image explaningImage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetExText();

        StartCoroutine(TextFadeIn());
        explaningText.transform.DOMoveY(-4, 0.25f);
        explaningImage.transform.DOMoveY(-4, 0.25f);
        StartCoroutine(AddScale());
    }

    private void SetExText()
    {
        //GameObject currentBTN = EventSystem.current.currentSelectedGameObject;

        //string buttonName = currentBTN.name;

        //Debug.Log(buttonName);

        //switch(buttonName)
        //{
        //    case storyButton.:
        //        Debug.Log("¾È³ç");
        //        break;
        //}
    }

    IEnumerator TextFadeIn()
    {
        Color alpha = new Color(0,64,0);

        while (alpha.a <= 1)
        {
            alpha.a += 0.04f;
            explaningImage.color = alpha;
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator AddScale()
    {
        explaningText.transform.DOScale(1f, 0.4f);
        yield return new WaitForSeconds(0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(TextFadeOut());
        explaningText.transform.DOMoveY(-7, 0.25f);
        explaningImage.transform.DOMoveY(-7, 0.25f);
        StartCoroutine(MinScale());
    }

    IEnumerator TextFadeOut()
    {
        Color alpha = new Color(0, 64, 0);

        while (alpha.a >= 0)
        {
            alpha.a += 0.04f;
            explaningImage.color = alpha;
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator MinScale()
    {
        explaningText.transform.DOScale(0.1f, 0.4f);
        yield return new WaitForSeconds(0.5f);
    }
}
