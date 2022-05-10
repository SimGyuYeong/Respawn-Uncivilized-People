using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ExplanaingText : MonoBehaviour
{
    private TextMeshProUGUI _expText;
    private Sequence _seq = null;

    private void Awake()
    {
        _expText = transform.Find("ExplanationText").GetComponent<TextMeshProUGUI>();
    }


    /// <summary>
    /// 테스트용 통신 함수
    /// </summary>
    /// <param name="i"></param>
    public void TestInterface(int i)
    {
        Debug.Log(i);
    }

    public void ShowText(string msg)
    {
        //_openCount++;
        //if (_openCount > 1)
        //{
        //    StopAllCoroutines();
        //    if (time > 0)
        //    {
        //        _openCount--;
        //        StartCoroutine(CloseCoroutine(time));
        //    }
        //    return;
        //}

        _expText.SetText(msg);
        DOTween.Kill(transform);
        if (_seq != null) _seq.Kill();

        transform.localScale = Vector3.zero;
        _seq = DOTween.Sequence();
        _seq.Append(transform.DOScale(Vector3.one * 1.2f, 0.3f));
        _seq.Append(transform.DOScale(Vector3.one * 0.9f, 0.1f));
        _seq.Append(transform.DOScale(Vector3.one, 0.1f));

        //if (time > 0)
        //{
        //    _seq = DOTween.Sequence();
        //    _seq.Append(transform.DOScale(Vector3.one * 1.2f, 0.1f));
        //    _seq.Append(transform.DOScale(Vector3.zero, 0.3f));
        //}
    }

    public void CloseText()
    {
        _seq = DOTween.Sequence();
        _seq.Append(transform.DOScale(Vector3.one * 1.2f, 0.1f));
        _seq.Append(transform.DOScale(Vector3.zero, 0.3f));
    }
}
