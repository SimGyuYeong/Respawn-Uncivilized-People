using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MemorialManager : MonoBehaviour
{
    public GameObject itemButton;

    public List<MemorialText> itemName = new List<MemorialText>();
    private TextMeshProUGUI[] _itemText = new TextMeshProUGUI[50];
    private GameObject _buttonInstansiateTransform;

    public Image _titleImage;
    public TextMeshProUGUI _memorialTitle;
    public TextMeshProUGUI _wordText;

    public TextMeshProUGUI _textExplan;

    private List<Button> _textButton = new List<Button>();

    public MemorialData _memorialData;
    public Image _textImage;

    private Sequence seq;

    public Image wordImage;

    IEnumerator SetText(int num)
    {
        _wordText.text = string.Format("");

        if (itemName[num].isUnlock)
        {
            while (wordImage.fillAmount < 1)
            {
                wordImage.fillAmount += 0.05f;
                yield return new WaitForSecondsRealtime(0.01f);
            }

            _wordText.text = string.Format(itemName[num].text);

            for (int i = 0; i <= _memorialData.wordMeaning[num].Length; i++)
            {
                _textExplan.text = itemName[i].isUnlock ? string.Format("{0}", _memorialData.wordMeaning[num].Substring(0, i)) : string.Format("아직 개방되지 않았습니다.");
                yield return new WaitForSecondsRealtime(0.5f / _memorialData.wordMeaning[num].Length);
            }
        }

        else
        {
            _textExplan.text = string.Format("아직 개방되지 않았습니다.");
        }
    }

    private void Start()
    {
        _buttonInstansiateTransform = transform.GetChild(0).Find("WORDPANEL").GetChild(0).GetChild(0).GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        StartCoroutine(SetTitle());
    }
    private void OnDisable()
    {
        DisablingMemorial();
    }

    public void InputText(int i)
    {
        wordImage.fillAmount = 0;

        seq = DOTween.Sequence();
        seq.Append(_textImage.DOFade(0, 0.12f)).SetUpdate(true);
        seq.AppendCallback(() => _textImage.sprite = _memorialData.wordImage[i]).SetUpdate(true);
        seq.Append(_textImage.DOFade(1, 0.1f)).SetUpdate(true);

        StopCoroutine(nameof(SetText));
        StartCoroutine("SetText", i);
    }

    IEnumerator SetTitle()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        seq = DOTween.Sequence();

        seq.Append(_titleImage.transform.DOLocalMoveX(0, 0.6f)).SetUpdate(true);
        seq.AppendInterval(0.14f).SetUpdate(true);
        seq.Append(_memorialTitle.transform.DOLocalMoveX(0, 0.4f)).SetUpdate(true);
        seq.AppendCallback(() => StartCoroutine(CreateButton())).SetUpdate(true);
    }

    IEnumerator CreateButton()
    {
        if (_textButton.Count != itemName.Count)
        {
            for (int i = 0; i < itemName.Count; i++)
            {
                int idx = i;

                GameObject _itemButton = Instantiate(itemButton, _buttonInstansiateTransform.transform);
                _textButton.Add(_itemButton.GetComponent<Button>());

                _itemText[i] = _textButton[i].transform.Find("Text").GetComponent<TextMeshProUGUI>();

                _textButton[i].onClick.AddListener(() => InputText(idx));

                _itemText[i].text = itemName[i].isUnlock ? string.Format($"{itemName[i].text}") : _itemText[i].text = string.Format("???");

                yield return new WaitForSecondsRealtime(0.06f);
            }
        }

        else
        {
            for (int i = 0; i < itemName.Count; i++)
            {
                _textButton[i].gameObject.SetActive(true);
                _itemText[i].text = itemName[i].isUnlock ? string.Format($"{itemName[i].text}") : _itemText[i].text = string.Format("???");
                yield return new WaitForSecondsRealtime(0.06f);
            }
        }
    }

    private void DisablingMemorial()
    {
        _titleImage.transform.position = new Vector2(-2028, _titleImage.transform.position.y);
        _memorialTitle.transform.position = new Vector2(-1950, _memorialTitle.transform.position.y);

        for (int i = 0; i < itemName.Count; i++)
        {
            _textButton[i].gameObject.SetActive(false);
        }
    }
}