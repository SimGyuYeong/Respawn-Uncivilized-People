using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    private const string URL = "https://docs.google.com/spreadsheets/d/18d1eO7_f3gewvcBi5MIe0sqh50lp1PF-kkQg2nm03wg/export?format=tsv";
    private Dictionary<int, string[,]> Sentence = new Dictionary<int, string[,]>();

    public GameObject textPanel;
    private Text _text;
    private GameObject _textEndObj;
    private Button _textNextButton;

    private List<string> _tutorialText = new List<string>();

    private int _number = 0, _lineSize = 0;
    private bool _isTyping = false, _isSkip = false;

    private void Awake()
    {
        _text = textPanel.transform.Find("text").GetComponent<Text>();
        _textEndObj = textPanel.transform.Find("textEnd").gameObject;
        _textNextButton = textPanel.transform.Find("Button").GetComponent<Button>();

        StartCoroutine(LoadTextData());
    }

    public IEnumerator LoadTextData()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        string[] line = data.Split('\n');
        int lineSize = line.Length;
        int rawSize;
        int chatID = 1, lineCount = 1, i, j;

        for (i = 1; i < lineSize; i++)
        {
            string[] row = line[i].Split('\t');
            rawSize = line[i].Split('\t').Length;
            if (row[0] != "")
            {
                if (chatID == 4)
                    _lineSize = lineCount;
                lineCount = 1;
                chatID = Convert.ToInt32(row[0]);
                Sentence[chatID] = new string[lineSize, 20];
            }

            if (chatID == 4)
            {
                for (j = 1; j < rawSize; j++)
                {
                    Sentence[chatID][lineCount, j] = row[j].Trim();
                }
                Sentence[chatID][lineCount, 19] = j.ToString();

                _tutorialText.Add(Sentence[chatID][lineCount, 2]);

                Sentence[chatID][lineCount, ++j] = "x";
                lineCount++;
            }
        }
    }

    public IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(0.5f);

        _textNextButton.enabled = false;

        Sequence seq = DOTween.Sequence();
        seq.Append(textPanel.transform.DOLocalMoveY(-350f, 1f));
        seq.AppendCallback(() =>
        {
            _textNextButton.enabled = true;
            StartCoroutine(TypingCoroutine());
        });
    }

    public IEnumerator TypingCoroutine()
    {
        _textEndObj.SetActive(false);
        _isTyping = true;

        for (int i = 0; i < _tutorialText[_number].Length + 1; i++)
        {

            if (_isSkip)
            {
                _text.text = string.Format(_tutorialText[_number]);           // 텍스트 넘김.....누르면 한줄이 한번에 딱
                _isSkip = false;
                break;
            }


            _text.text = string.Format(_tutorialText[_number].Substring(0, i));
            yield return new WaitForSeconds(0.1f);

            _text.text = string.Format(_tutorialText[_number]);
        }

        ///↓↓↓ 타이핑 끝난 후 실행
        _textEndObj.SetActive(true);
        _isTyping = false;
    }

    public void SkipTextClick()
    {
        if(_isTyping)
        {
            _isSkip = true;
        }
        else
        {
            _number++;
            if(_number >= _lineSize - 1)
            {
                Sequence seq = DOTween.Sequence();
                seq.Append(textPanel.transform.DOLocalMoveY(-700f, 1f));
                seq.AppendCallback(() =>
                {
                    FightManager.Instance.turnType = FightManager.TurnType.Player_Wait;
                    FightManager.Instance.TurnChange();
                    FightManager.Instance.isIng = false;
                    FightManager.Instance.isTutorial = false;
                });
            }
            else
            {
                StartCoroutine(TypingCoroutine());
            }
        }
    }
}
