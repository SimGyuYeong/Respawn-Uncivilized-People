using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextUIManager : MonoBehaviour
{
    [Tooltip("넘엉가는 화면 이미지")]
    [SerializeField] private GameObject _loadingSprite;

    #region 설정창
    [Header("설정창")]
    [Tooltip("설정창 UI")]
    [SerializeField] private GameObject _optionUI;

    private GameObject _defaultOption;
    private GameObject _soundOption;
    public GameObject _memorialOption;

    public Color selectColor;
    private TextMeshProUGUI _memorialText;
    private TextMeshProUGUI _defaultText;
    private TextMeshProUGUI _soundText;
    private TextMeshProUGUI _goTitleText;
    private TextMeshProUGUI _backText;
    #endregion

    public bool isLoading = false;

    public static TextUIManager instance;

    private void Awake()
    {
        instance = this;

        _defaultOption = _optionUI.transform.Find("DefaultSetting").gameObject;
        _soundOption = _optionUI.transform.Find("SoundSetting").gameObject;
        //_memorialOption = _optionUI.transform.Find("Memorial").gameObject;

        _memorialText = _optionUI.transform.Find("Memorial").GetComponent<TextMeshProUGUI>();
        _defaultText = _optionUI.transform.Find("Default").GetComponent<TextMeshProUGUI>();
        _soundText = _optionUI.transform.Find("Sound").GetComponent<TextMeshProUGUI>();
        _goTitleText = _optionUI.transform.Find("GoTitle").GetComponent<TextMeshProUGUI>();
        _backText = _optionUI.transform.Find("Back").GetComponent<TextMeshProUGUI>();

        SetMemorialPanel();
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

    public void ShowOptionPanel(bool check)
    {
        _optionUI.SetActive(check);
        if (check == true) Time.timeScale = 0f;
        else
        {
            Time.timeScale = 1f;
            _goTitleText.color = Color.gray;
            _backText.color = Color.gray;
        }
    }

    public void DefaultOptionClick()
    {
        _soundOption.SetActive(false);
        _defaultOption.SetActive(true);
        _memorialOption.SetActive(false);
        _soundText.color = Color.gray;
        _memorialText.color = Color.gray;
        _defaultText.color = selectColor;
    }

    public void SoundOptionClick()
    {
        _soundOption.SetActive(true);
        _defaultOption.SetActive(false);
        _memorialOption.SetActive(false);
        _soundText.color = selectColor;
        _memorialText.color = Color.gray;
        _defaultText.color = Color.gray;
    }

    public void MemorialonClick()
    {
        _soundOption.SetActive(false);
        _defaultOption.SetActive(false);
        _memorialOption.SetActive(true);
        _soundText.color = Color.gray;
        _memorialText.color = selectColor;
        _defaultText.color = Color.gray;
    }

    private void SetMemorialPanel()
    {
        Instantiate(_memorialOption, _optionUI.transform);
    }
}
