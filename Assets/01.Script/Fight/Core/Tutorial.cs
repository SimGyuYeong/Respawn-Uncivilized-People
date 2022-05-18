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
    private bool _isTyping = false;

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
                if (chatID == 3)
                    _lineSize = lineCount;
                lineCount = 1;
                chatID = Convert.ToInt32(row[0]);
                Sentence[chatID] = new string[lineSize, 20];
            }

            if (chatID == 3)
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
            TextTyping();
        });
    }

    public void TextTyping()
    {
        _isTyping = true;
        _text.text = "";
        _textEndObj.SetActive(false);

        Sequence seq = DOTween.Sequence();
        seq.Append(_text.DOText(_tutorialText[_number], 3f));
        seq.AppendCallback(() =>
        {
            _textEndObj.SetActive(true);
            _isTyping = false;
        });
    }

    public void SkipTextClick()
    {
        if(_isTyping)
        {
            DOTween.KillAll();

            _text.text = _tutorialText[_number];
            _textEndObj.SetActive(true);
            _isTyping = false;
        }
        else
        {
            _number++;
            if(_number >= _lineSize - 1)
            {
                DOTween.KillAll();

                Sequence seq = DOTween.Sequence();
                seq.Append(textPanel.transform.DOLocalMoveY(-700f, 1f));
                seq.AppendCallback(() =>
                {
                    FightManager.Instance.turnType = FightManager.TurnType.Wait_Player;
                    FightManager.Instance.TurnChange();
                    FightManager.Instance.OnUIChange?.Invoke();
                    FightManager.Instance.isIng = false;
                });
            }
            else
            {
                TextTyping();
            }
        }
    }
}
