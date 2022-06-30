using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MemorialManager : MonoBehaviour
{
    public GameObject itemButton;
    public List<string> itemName = new List<string>();
    private TextMeshProUGUI[] _itemText = new TextMeshProUGUI[50];
    private GameObject _buttonInstansiateTransform;

    public Image _titleImage;
    public TextMeshProUGUI _memorialTitle;
    public TextMeshProUGUI _wordText;

    public TextMeshProUGUI _textExplan;

    private Button[] _textButton = new Button[50];

    public MemorialData _memorialData;
    public Image _textImage;

    private Sequence seq;

    public Image wordImage;

    IEnumerator SetText(int num)
    {
        _wordText.text = string.Format("");

        while (wordImage.fillAmount < 1)
        {
            wordImage.fillAmount += 0.05f;
            yield return new WaitForSeconds(0.01f);
        }

        _wordText.text = string.Format(itemName[num]);

        for (int i = 0; i <= _memorialData.wordMeaning[num].Length; i++)
        {
            _textExplan.text = string.Format("{0}", _memorialData.wordMeaning[num].Substring(0, i));
            yield return new WaitForSeconds(0.5f / _memorialData.wordMeaning[num].Length);
        }
    }

    private void Start()
    {
        StartCoroutine(SetTitle());
        _buttonInstansiateTransform = transform.GetChild(0).Find("WORDPANEL").GetChild(0).GetChild(0).GetChild(0).gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CreateButton());
        }
    }

    public void InputText(int i)
    {
        wordImage.fillAmount = 0;

        seq = DOTween.Sequence();
        seq.Append(_textImage.DOFade(0, 0.12f));
        seq.AppendCallback(() => _textImage.sprite = _memorialData.wordImage[i]);
        seq.Append(_textImage.DOFade(1, 0.1f));

        StopCoroutine(nameof(SetText));
        StartCoroutine("SetText", i);
    }

    IEnumerator SetTitle()
    {
        yield return new WaitForSeconds(1.5f);

        seq = DOTween.Sequence();

        seq.Append(_titleImage.transform.DOLocalMoveX(0, 0.6f));
        seq.AppendInterval(0.2f);
        seq.Append(_memorialTitle.transform.DOLocalMoveX(0, 0.4f));
    }

    IEnumerator CreateButton()
    {
        for (int i = 0; i < itemName.Count; i++)
        {
            int idx = i;

            GameObject _itemButton = Instantiate(itemButton, _buttonInstansiateTransform.transform);

            _textButton[i] = _itemButton.GetComponent<Button>();

            _itemText[i] = _textButton[i].transform.Find("Text").GetComponent<TextMeshProUGUI>();

            _textButton[i].onClick.AddListener(() => InputText(idx));

            _itemText[i].text = string.Format($"{itemName[i]}");

            yield return new WaitForSeconds(0.06f);
        }
    }
}