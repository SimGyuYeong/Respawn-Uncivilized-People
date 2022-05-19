using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class FreeTimeDirect : MonoBehaviour
{
    public static FreeTimeDirect Instance;

    public FreeTimeText freeTimeText;

    private Camera maincam;

    private Image textpanelImage;

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

    private void Start()
    {
        StartCoroutine(Init()); 

        //textpanelImage = freeTimeText.textPanelObj.GetComponent<Image>();
    }
    IEnumerator Init()
    {
        GameObject g = TextManager.Instance.textPanelObj;
        while (g == null)
        {
            g = TextManager.Instance.textPanelObj;
            yield return null; 
        }
        textpanelImage = TextManager.Instance.textPanelObj.GetComponent<Image>();
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
        freeTimeText.storyPanel.transform.DOScale(1.35f, 2.3f);
        _seq.Join(maincam.transform.DOMoveY(0.76f, 0.44f)).SetEase(Ease.OutQuad);
        _seq.Append(maincam.transform.DOMoveY(0, 0.34f)).SetEase(Ease.OutSine);
        _seq.Append(maincam.transform.DOMoveY(0.76f, 0.44f)).SetEase(Ease.OutQuad);
        _seq.Append(maincam.transform.DOMoveY(0f, 0.34f)).SetEase(Ease.OutSine);
        _seq.Append(maincam.transform.DOMoveY(0.76f, 0.44f)).SetEase(Ease.OutQuad);
        _seq.Append(maincam.transform.DOMoveY(0f, 0.34f)).SetEase(Ease.OutSine);
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
        //Debug.Log("aassdafafsasfbsestmsrsrsrmsrymsmsym");
        //FreeTimeText.Instance._endAnimationObj.SetActive(false);
        while (textpanelImage.color.a >= 0)
        {
            Color alpha = textpanelImage.color;
            alpha.a -= 0.05f;
            textpanelImage.color = alpha;
            yield return new WaitForSeconds(0.007f);
        }
        textpanelImage.gameObject.SetActive(false);
    }

    public void LookAround(Action action = null)
    {
        Sequence _seq = DOTween.Sequence();
        StartCoroutine(FadeInTextPanel());
        _seq.Append(freeTimeText.storyPanel.transform.DOScale(1.35f, 1.2f)).SetEase(Ease.OutSine);
        _seq.Append(maincam.transform.DOMoveX(3f, 1f)).SetEase(Ease.Linear);
        _seq.Append(maincam.transform.DOMoveX(-3f, 1.8f)).SetEase(Ease.Linear);
        _seq.Append(maincam.transform.DOMoveX(0, 1f)).SetEase(Ease.Linear);
        _seq.Append(freeTimeText.storyPanel.transform.DOScale(1f, 1.2f)).SetEase(Ease.OutSine);
        _seq.AppendCallback(FadeOutTextPanel);
        _seq.AppendInterval(0.5f);
        if(action!=null)
        {
            _seq.AppendCallback(() =>
            {
                action.Invoke();
            });
        }
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
            alpha.a += 0.01f;
            textpanelImage.color = alpha;
            yield return new WaitForSeconds(0.007f);
        }
        //freeTimeText.textPanelObj.transform.Find("textEnd").gameObject.SetActive(true);
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
            yield return new WaitForSeconds(0.007f);
        }
    }

    public void ZoomIn()
    {
        //StartCoroutine(FadeInTextPanel());
        freeTimeText.storyPanel.transform.DOScale(1.35f, 1.2f);
    }

    public void ZoomOut()
    {
        freeTimeText.storyPanel.transform.DOScale(1f, 0.9f);
        //StartCoroutine(FadeOutCo());
    }
}
