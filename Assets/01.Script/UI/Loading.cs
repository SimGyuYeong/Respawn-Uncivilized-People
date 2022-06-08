using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Loading : MonoBehaviour
{
    public Image textImage;
    public Image textBackgroundImage;
    public TextMeshPro textMeshPro;

    void Start()
    {
        if (textImage == null || textBackgroundImage == null || textMeshPro == null)
        {
            textBackgroundImage = transform.Find("TextBackground/Image").GetComponent<Image>();
            textImage = transform.Find("LoadingText/Image").GetComponent<Image>();
            textMeshPro = transform.Find("LoadingText").GetComponent<TextMeshPro>();
        }
        LoadingAni();
    }

    private void LoadingAni()
    {
        Sequence seq = DOTween.Sequence();

        textImage.transform.DOMoveX(2060, 1.5f).SetEase(Ease.InOutQuart).SetLoops(-1, LoopType.Restart);
        textBackgroundImage.transform.DOMoveX(2060, 1.5f).SetEase(Ease.InOutQuart).SetLoops(-1, LoopType.Restart);

        //StartCoroutine(MoveText());
    }

    //IEnumerator MoveText()
    //{
        //for(int i =0;i<textMeshPro.)
    //}
}