using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FreeTimeDirect : MonoBehaviour
{
    public static FreeTimeDirect Instance;

    public FreeTimeText freeTimeText;

    private Camera maincam;

    public Image textpanelImage;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("¿À·ù");
        }
        Instance = this;

        maincam = Camera.main;
        freeTimeText = FindObjectOfType<FreeTimeText>();
    }

    //public void FadeIn()
    //{
    //    StartCoroutine(Fade());
    //}

    public void Walking()
    {
        Sequence _seq = DOTween.Sequence();
        Debug.Log("a");
        StartCoroutine(FadeInTextPanel());
        freeTimeText.storyPanel.transform.DOScale(1.3f, 2f);
        _seq.Join(maincam.transform.DOMoveY(0.7f, 0.4f));
        _seq.Append(maincam.transform.DOMoveY(0, 0.25f));
        _seq.Append(maincam.transform.DOMoveY(0.7f, 0.4f));
        _seq.Append(maincam.transform.DOMoveY(0f, 0.25f));
        _seq.Append(maincam.transform.DOMoveY(0.7f, 0.4f));
        _seq.Append(maincam.transform.DOMoveY(0f, 0.25f));
        //_seq.Append(freeTimeText.storyPanel.transform.DOScale(1f, 3.5f)).SetEase(Ease.Flash);
        //freeTimeText.storyPanel.transform.DOScale(2f, 2f);
        //yield return new WaitForSeconds(0.8f);
    }

    public void FadeInText()
    {
        StartCoroutine(FadeInTextPanel());
    }

    public IEnumerator FadeInTextPanel()
    {
        while (textpanelImage.color.a >= 0)
        {
            Color alpha = textpanelImage.color;
            alpha.a -= 0.05f;
            textpanelImage.color = alpha;
            yield return new WaitForSeconds(0.007f);
        }
        textpanelImage.gameObject.SetActive(true);
    }

    public void FadeOutTextPanel()
    {
        StartCoroutine(FadeOutCo()); 
    }
    IEnumerator FadeOutCo()
    {
        textpanelImage.gameObject.SetActive(true);
        while (textpanelImage.color.a <= 1)
        {
            Color alpha = textpanelImage.color;
            alpha.a += 0.05f;
            textpanelImage.color = alpha;
            yield return new WaitForSeconds(0.008f);
        }
    }
    
    public void FadeOut()
    {
        StartCoroutine(FadeOutScreen());
    }
    public void FadeIn()
    {
        StartCoroutine(FadeInScreen());
    }

    IEnumerator FadeInScreen()
    {
        Color color = ButtonManager.Instance.fadeImage.color;
        while (ButtonManager.Instance.fadeImage.color.a >= 0)
        {
            color.a -= 0.01f;
            ButtonManager.Instance.fadeImage.color = color;
            yield return new WaitForSeconds(0.007f);
        }
        ButtonManager.Instance.fadeImage.gameObject.SetActive(false);
    }
        
    IEnumerator FadeOutScreen()
    {
        Color color = ButtonManager.Instance.fadeImage.color;
        ButtonManager.Instance.fadeImage.gameObject.SetActive(true);
        while (ButtonManager.Instance.fadeImage.color.a <= 1)
        {
            color.a += 0.01f;
            ButtonManager.Instance.fadeImage.color = color;
            yield return new WaitForSeconds(0.008f);
        }
    }

}
