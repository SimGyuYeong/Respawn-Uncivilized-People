using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
    const string URL = "https://docs.google.com/spreadsheets/d/18d1eO7_f3gewvcBi5MIe0sqh50lp1PF-kkQg2nm03wg/export?format=tsv";

    [SerializeField] private Text textPanel;
    [SerializeField] private Transform selectPanel;
    [SerializeField] private GameObject selectButton;
    [SerializeField] private GameObject[] background;
    [SerializeField] private GameObject[] image;

    Dictionary<int, string[,]> Sentence = new Dictionary<int, string[,]>();
    Dictionary<int, int> max = new Dictionary<int, int>();
    List<string> select = new List<string>();

    public int chatID = 1, typingID = 1, imageID = 1, backID = 1;
    bool isTyping = false, skip = false;

    IEnumerator Start()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        string[] line = data.Split('\n');
        int lineSize = line.Length;
        int rawSize = line[0].Split('\t').Length;
        int chatID = 1, lineCount = 1, i, j;

        for(i = 1; i < lineSize; i++)
        {
            string[] row = line[i].Split('\t');
            if(row[0] != "")
            {
                lineCount = 1;
                chatID = System.Convert.ToInt32(row[0]);
                max[chatID] = 1;
                Sentence[chatID] = new string[lineSize, 20];
            }

            for(j = 1; j < rawSize; j++)
            {
                Sentence[chatID][lineCount, j] = row[j];
            }
            Sentence[chatID][lineCount, 19] = j.ToString();

            max[chatID]++;
            lineCount++;
        }
    }

    public IEnumerator Typing()
    {
        textPanel.gameObject.SetActive(true);
        isTyping = true;
        if (Sentence[chatID][typingID, 3] != "")
        {
            backID = System.Convert.ToInt32(Sentence[chatID][typingID, 3]) - 1;
            background[backID].SetActive(true);
        }
        if (Sentence[chatID][typingID, 4] != "")
        {
            imageID = System.Convert.ToInt32(Sentence[chatID][typingID, 4]) - 1;
            image[imageID].SetActive(true);
        }
        for (int i = 0; i < Sentence[chatID][typingID, 2].Length + 1; i++)
        {
            if (skip)
            {
                textPanel.text = string.Format("{0}\n{1}", Sentence[chatID][typingID, 1], Sentence[chatID][typingID, 2]);
                skip = false;
                break;
            }
            textPanel.text = string.Format("{0}\n{1}", Sentence[chatID][typingID, 1], Sentence[chatID][typingID, 2].Substring(0, i));
            yield return new WaitForSeconds(0.1f);
        }
        isTyping = false;
    }

    public void SkipText()
    {
        if (!isTyping)
        {
            string eventName = Sentence[chatID][typingID, 5];
            if (eventName == "이동")
            {
                chatID = System.Convert.ToInt32(Sentence[chatID][typingID, 6]);
                typingID = 0;
            }

            if (eventName == "함수")
            {
                Invoke(Sentence[chatID][typingID, 6], 0f);
            }
            if (backID >= 1) background[backID].SetActive(false);
            if (imageID >= 1) image[imageID].SetActive(false);

            if (eventName == "선택")
            {
                textPanel.gameObject.SetActive(false);
                SelectOpen();
            }

            else
            {
                typingID++;
                if (typingID != max[chatID]) StartCoroutine(Typing());
                else textPanel.gameObject.SetActive(false);
            }
        }
        else skip = true;
    }

    public void TextMove()
    {
        int num = Random.Range(6, System.Convert.ToInt32(Sentence[chatID][typingID, 19]));
        chatID = System.Convert.ToInt32(Sentence[chatID][typingID, num]);
    }

    public void SelectOpen()
    {
        for(int i = 6; i < System.Convert.ToInt32(Sentence[chatID][typingID, 19]); i++)
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
        for(int i = 1; i < selectPanel.transform.childCount; i++)
        {
            if(selectPanel.transform.GetChild(i).gameObject == selectObj)
            {
                int num = (i - 1) * 2;
                chatID = System.Convert.ToInt32(select[num+1]);
                selectPanel.gameObject.SetActive(false);
                textPanel.gameObject.SetActive(true);
                typingID = 1;
                StartCoroutine(Typing());
            }
        }
    }

    private void GoToMinimap()
    {
        SceneManager.LoadScene("MiniMap");   
    }
}
