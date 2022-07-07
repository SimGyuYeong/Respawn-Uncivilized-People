using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TextUIManager : MonoBehaviour
{
    [Tooltip("넘엉가는 화면 이미지")]
    [SerializeField] private GameObject _loadingSprite;

    #region 설정창
    [Header("설정창")]
    [Tooltip("설정창 UI")]
    [SerializeField] private GameObject _optionUI;

    public GameObject textLogUI;
    private GameObject _defaultOption;
    private GameObject _soundOption;
    public GameObject _memorialOptionPrefab;
    public GameObject _memorialOption;

    public Color selectColor;
    private TextMeshProUGUI _memorialText;
    private TextMeshProUGUI _defaultText;
    private TextMeshProUGUI _soundText;
    private TextMeshProUGUI _goTitleText;
    private TextMeshProUGUI _backText;

    private Slider _bgmSlider;
    private Slider _effectSlider;
    #endregion

    public bool isLoading = false;
    public bool isMemorial = false;

    public static TextUIManager instance;

    private void Awake()
    {
        instance = this;

        textLogUI = _optionUI.transform.parent.Find("TextLogPanel").gameObject;
        _defaultOption = _optionUI.transform.Find("DefaultSetting").gameObject;
        _soundOption = _optionUI.transform.Find("SoundSetting").gameObject;
        //_memorialOption = _optionUI.transform.Find("Memorial").gameObject;

        _memorialText = _optionUI.transform.Find("Memorial").GetComponent<TextMeshProUGUI>();
        _defaultText = _optionUI.transform.Find("Default").GetComponent<TextMeshProUGUI>();
        _soundText = _optionUI.transform.Find("Sound").GetComponent<TextMeshProUGUI>();
        _goTitleText = _optionUI.transform.Find("GoTitle").GetComponent<TextMeshProUGUI>();
        _backText = _optionUI.transform.Find("Back").GetComponent<TextMeshProUGUI>();

        _bgmSlider = _optionUI.transform.Find("SoundSetting/BGM/Slider").GetComponent<Slider>();
        _effectSlider = _optionUI.transform.Find("SoundSetting/Effect/Slider").GetComponent<Slider>();

        _bgmSlider.value = PlayerPrefs.GetFloat("BGM", 0);
        _effectSlider.value = PlayerPrefs.GetFloat("Effect", 0);

        SetMemorialPanel();
    }

    public void Loading(Action action)
    {
        isLoading = true;

        Sequence loadingSeq = DOTween.Sequence();
        loadingSeq.Append(_loadingSprite.transform.DOLocalMoveY(0f, 0.35f).SetEase(Ease.InQuart)).SetUpdate(true);
        loadingSeq.AppendCallback(() => action?.Invoke()).SetUpdate(true);
        loadingSeq.Append(_loadingSprite.transform.DOLocalMoveY(1090, 0.35f).SetEase(Ease.OutQuart)).SetUpdate(true);
        loadingSeq.AppendCallback(() =>
        {
            isLoading = false;
            loadingSeq.Rewind();
        }).SetUpdate(true);
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
        isMemorial = true;

        _soundText.color = Color.gray;
        _memorialText.color = selectColor;
        _defaultText.color = Color.gray;

        Loading(() =>
        {
            _memorialOption.SetActive(true); 
            _soundOption.SetActive(false);
            _defaultOption.SetActive(false);
        });
    }

    public void DownMemorial()
    {
        _soundText.color = Color.gray;
        _memorialText.color = Color.gray;
        _defaultText.color = selectColor;

        Loading(() =>
        {
            _memorialOption.SetActive(false);
            _soundOption.SetActive(false);
            _defaultOption.SetActive(true);
        });

        isMemorial = false;
    }

    private void SetMemorialPanel()
    {
        GameObject _memorial = _memorialOptionPrefab;
        _memorialOption = Instantiate(_memorial).transform.GetChild(0).gameObject;
        DontDestroyOnLoad(_memorialOption);
    }
}
