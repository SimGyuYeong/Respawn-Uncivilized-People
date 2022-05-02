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
        autoCheckText.text = "�ڵ����� ����";
        Debug.Log("¥��");
        //checker.DOFade(1, 0.5f);
        //mySequence.Append(checker.DOFade(0, 0.5f));
        gameObject.SetActive(false);
    }

    public void OffChecker()
    {
        gameObject.SetActive(true);
        autoCheckText.text = "�ڵ����� ����";
        //checker.DOFade(1, 0.5f);
        //mySequence.Append(checker.DOFade(0, 0.5f));
        gameObject.SetActive(false);
    }
}
