using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System;


public class TextManager : MonoBehaviour
{
    const string URL = "https://docs.google.com/spreadsheets/d/18d1eO7_f3gewvcBi5MIe0sqh50lp1PF-kkQg2nm03wg/export?format=tsv";

    [SerializeField] private GameObject textImage;
    [SerializeField] private Text textPanel;
    [SerializeField] private Transform selectPanel;
    [SerializeField] private GameObject selectButton;
    [SerializeField] private GameObject[] background;
    [SerializeField] private GameObject[] image;
    [SerializeField] private GameObject endObject;

    List<string> textLog = new List<string>();
    Dictionary<int, string[,]> Sentence = new Dictionary<int, string[,]>();
    Dictionary<int, int> max = new Dictionary<int, int>();
    List<string> select = new List<string>();

    public int chatID = 1, typingID = 1, backID = 1, slotID = 0;
    private bool isTyping = false, skip = false;
    string[] imageList;
    public float chatSpeed = 0.1f;
    public bool Auto = false;

    private RectTransform _targetTransform;

    private void Awake()
    {
        StartCoroutine(LoadTextData());
        _targetTransform = textPanel.gameObject.GetComponent<RectTransform>();
    }

    IEnumerator LoadTextData()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        string[] line = data.Split('\n');
        int lineSize = line.Length;
        int rawSize = line[0].Split('\t').Length;
        int chatID = 1, lineCount = 1, i, j;

        for (i = 1; i < lineSize; i++)
        {
            string[] row = line[i].Split('\t');
            if (row[0] != "")
            {
                lineCount = 1;
                chatID = Convert.ToInt32(row[0]);
                max[chatID] = 1;
                Sentence[chatID] = new string[lineSize, 20];
            }

            for (j = 1; j < rawSize; j++)
            {
                Sentence[chatID][lineCount, j] = row[j];
            }
            Sentence[chatID][lineCount, 19] = j.ToString();
            Sentence[chatID][lineCount, ++j] = "x";
            max[chatID]++;
            lineCount++;
        }
    }

    public IEnumerator Typing()
    {
        textImage.SetActive(true);
        if (!Auto) endObject.SetActive(false);
        isTyping = true;
        if (Sentence[chatID][typingID, 3] != "")
        {
            backID = Convert.ToInt32(Sentence[chatID][typingID, 3]) - 1;
            background[backID].SetActive(true);
        }
        if (Sentence[chatID][typingID, 4] != "") imageSetactive(true);
        for (int i = 0; i < Sentence[chatID][typingID, 2].Length + 1; i++)
        {
            if (skip)
            {
                textPanel.text = string.Format("{0}\n{1}", Sentence[chatID][typingID, 1], Sentence[chatID][typingID, 2]);
                skip = false;
                break;
            }
            textPanel.text = string.Format("{0}\n{1}", Sentence[chatID][typingID, 1], Sentence[chatID][typingID, 2].Substring(0, i));
            yield return new WaitForSeconds(chatSpeed);
        }
        if (!Auto) endObject.SetActive(true);
        isTyping = false;

        textLog.Add(string.Format("{0}: {1}", Sentence[chatID][typingID, 1], Sentence[chatID][typingID, 2]));

        if (Auto)
        {
            yield return new WaitForSeconds(1f);
            SkipText();
        }
    }

    public void SkipTextClick()
    {
        if (!Auto)
        {
            if (!isTyping) SkipText();
            else skip = true;
        }
    }

    public void SkipText()
    {
        if (backID >= 1) background[backID].SetActive(false);
        if (Sentence[chatID][typingID, 4] != "") imageSetactive(false);

        string eventName = Sentence[chatID][typingID, 5];
        if (eventName == "함수") Invoke(Sentence[chatID][typingID, 6], 0f);
        else if (eventName == "이동")
        {
            int num = UnityEngine.Random.Range(6, Convert.ToInt32(Sentence[chatID][typingID, 19]));
            chatID = Convert.ToInt32(Sentence[chatID][typingID, num]);
            typingID = 0;
        }
        if (eventName == "선택")
        {
            if (Sentence[chatID][typingID, 3] != "")
            {
                backID = Convert.ToInt32(Sentence[chatID][typingID, 3]) - 1;
                background[backID].SetActive(true);
            }
            textImage.SetActive(false);
            SelectOpen();
        }
        else
        {
            typingID++;
            if (typingID != max[chatID]) StartCoroutine(Typing());
            else textImage.SetActive(false);
        }
    }

    public void SelectOpen()
    {
        for (int i = 6; i < Convert.ToInt32(Sentence[chatID][typingID, 19]); i++)
        {
            select.Add(Sentence[chatID][typingID, i]);
            GameObject button = Instantiate(selectButton, selectPanel);
            Text selectText = button.transform.GetChild(0).GetComponent<Text>();
            selectText.text = Sentence[chatID][typingID, i];
            select.Add(Sentence[chatID][typingID, ++i]);
            button.SetActive(true);
        }
        selectPanel.gameObject.SetActive(true);
    }

    public void Select(GameObject selectObj)
    {
        for (int i = 1; i < selectPanel.transform.childCount; i++)
        {
            if (selectPanel.transform.GetChild(i).gameObject == selectObj)
            {
                int num = (i - 1) * 2;
                chatID = Convert.ToInt32(select[num + 1]);
                selectPanel.gameObject.SetActive(false);
                textImage.SetActive(true);
                typingID = 1;
                StartCoroutine(Typing());
            }
        }
    }

    private void imageSetactive(bool set)
    {
        imageList = Sentence[chatID][typingID, 4].Split(',');
        foreach (string x in imageList)
        {
            image[Convert.ToInt32(x) - 1].SetActive(set);
        }
    }

    private void PlayMusic(int num)
    {
        GameManager.Instance.SOUND.PauseMusic();
        GameManager.Instance.SOUND.PlayingMusic(num, 0.5f);
    }

    private void CameraShaking()
    {
        Camera.main.GetComponent<CameraShaking>().ShakeForTime(0.2f);
    }

    public void AutoPlay()
    {
        Auto = !Auto;
        if (!isTyping) SkipText();
        endObject.SetActive(false);
    }

    public void ShowTextLog()
    {
        foreach(string text in textLog)
        {
            Debug.Log(text);
        }
    }
}
