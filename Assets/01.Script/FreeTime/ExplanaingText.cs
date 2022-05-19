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
        Vector2 position = new Vector2(0, -4.5f);
        _expText.SetText(msg);
        DOTween.Kill(transform);
        if (_seq != null) _seq.Kill();

        //transform.position = new Vector3(0, -3, 0);
        _seq = DOTween.Sequence();
        _seq.Append(transform.DOMove(position * 0.9f, 0.18f));
        _seq.Append(transform.DOMove(position, 0.1f));
    }

    public void CloseText()
    {
        Vector2 position = new Vector2(0, -8.6f);
        if (_seq != null) _seq.Kill();
        _seq = DOTween.Sequence();
        //_seq.Append(transform.DOMove(position * 1.1f, 0.08f));
        _seq.Append(transform.DOMove(position, 0.18f));
    }
}
