using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AutoChecker : MonoBehaviour
{
    [SerializeField]
    private Text autoCheckText;
    [SerializeField]
    private Material checker;
    private Sequence mySequence;

    public void Start()
    {
        mySequence = DOTween.Sequence();
        gameObject.SetActive(false);
        checker = GetComponent<Material>();
    }

    public void OnChecker()
    {
        gameObject.SetActive(true);
        autoCheckText.text = "자동진행 시작";
        Debug.Log("짜란");
        //checker.DOFade(1, 0.5f);
        //mySequence.Append(checker.DOFade(0, 0.5f));
        gameObject.SetActive(false);
    }

    public void OffChecker()
    {
        gameObject.SetActive(true);
        autoCheckText.text = "자동진행 중지";
        //checker.DOFade(1, 0.5f);
        //mySequence.Append(checker.DOFade(0, 0.5f));
        gameObject.SetActive(false);
    }
}
