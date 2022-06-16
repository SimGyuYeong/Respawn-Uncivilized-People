using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextUIManager : MonoBehaviour
{
    [Tooltip("�Ѿ����� ȭ�� �̹���")]
    [SerializeField] private GameObject _loadingSprite;

    [Tooltip("����â UI")]
    [SerializeField] private GameObject _optionUI;

    public bool isLoading = false;

    public static TextUIManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void Loading(Action action)
    {
        isLoading = true;

        Sequence seq = DOTween.Sequence();
        seq.Append(_loadingSprite.transform.DOMoveY(0.2f, 0.35f).SetEase(Ease.InQuart));
        seq.AppendCallback(() => action?.Invoke());
        seq.Append(_loadingSprite.transform.DOMoveY(10, 0.35f).SetEase(Ease.OutQuart));
        seq.AppendCallback(() =>
        {
            isLoading = false;
            seq.Rewind();
        });
    }
}
