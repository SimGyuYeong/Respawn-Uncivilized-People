using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
    const string URL = "https://docs.google.com/spreadsheets/d/18d1eO7_f3gewvcBi5MIe0sqh50lp1PF-kkQg2nm03wg/export?format=tsv";

    private TextDataSave _textDataSO;
    public TextDataSave TextSO => _textDataSO;

    [SerializeField] protected TextMeshPro _textPanel;
    public GameObject textPanelObj;

    [SerializeField] protected Transform selectPanel; 

    [SerializeField] protected GameObject _selectButton;
    public GameObject[] background; // ���� ���ȭ�� �迭
    public GameObject[] image;    // �߰��� ����� �̹��� 
    [SerializeField] protected GameObject _endAnimationObj; // �ؽ�Ʈ ������ �ؽ�Ʈ �г� ������ �Ʒ����� �ٿ˶ٿ� ��۹�� �ϴ� ��
    [SerializeField] protected float shakeTime = 0.13f;
    [SerializeField] protected float shakestr = 0;
    [SerializeField] protected GameObject textlogPrefab;
    [SerializeField] protected Transform textlogView;
    [SerializeField] private Text autoChecker;
    
    Dictionary<int, string[,]> Sentence = new Dictionary<int, string[,]>();
    protected Dictionary<int, int> max = new Dictionary<int, int>();
    protected List<string> select = new List<string>();

    public int chatID = 1, lineNumber = 1, backgroundID = 0, slotID = 0; // ID �⺻ �� ����
    protected bool isTyping = false, skip = false; // �ؽ�Ʈ ��ŵ �� �⺻ �� ����
    public float chatSpeed = 0.1f, autoSpeed = 1f; // �ؽ�Ʈ ������ �ӵ��� Auto �ӵ� �⺻ �� ����
    public bool isAuto = false;

    private static TextManager instance;

    public Action<GameObject> OnEffectObject;
    public UnityEvent TextTyping;
    public UnityEvent OnTextTypingEnd;

    private enum IDType
    {
        ChatID = 0,
        CharacterName,
        Text,
        BackgroundID,
        ImageID,
        Direct,
        SFX,
        BGM,
        Event
    }

    public static TextManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TextManager>();
                if (instance == null)
                {
                    GameObject container = new GameObject("TextManager");
                    instance = container.AddComponent<TextManager>();
                }
            }
            return instance;
        }
    }

    private void Update()
    {
        if(isAuto == true)
        {
            autoChecker.text = "<color=red>�ڵ�����</color>";
        }
        else
        {
            autoChecker.text = "�ڵ�����";
        }
    }

    private void Awake()
    {
        _textDataSO = GetComponentInChildren<TextDataSave>();
        StartCoroutine(LoadTextData(URL)); // �ؽ�Ʈ ������ �б�
        _textPanel = textPanelObj.transform.Find("text").GetComponent<TextMeshPro>();
    }

    public IEnumerator LoadTextData(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text, vertualText;
        string[] line = data.Split('\n'), vText;
        int lineSize = line.Length;
        int rawSize;
        int chatID = 1, lineCount = 1, i, j;

        for (i = 1; i < lineSize; i++)
        {
            string[] row = line[i].Split('\t');
            rawSize = line[i].Split('\t').Length;
            if (row[0] != "")
            {
                lineCount = 1;
                chatID = Convert.ToInt32(row[0]);
                max[chatID] = 1;
                Sentence[chatID] = new string[lineSize, 20];
            }

            for (j = 1; j < rawSize; j++)
            {
                Sentence[chatID][lineCount, j] = row[j].Trim();
            }
            Sentence[chatID][lineCount, 19] = j.ToString();

            vertualText = Sentence[chatID][lineCount, 2];

            if (vertualText != null)
            {
                foreach(char N in vertualText)
                {
                    if(N == 'N')
                    {
                        vText = vertualText.Split('N');
                        if (vText[1] != null)
                        {
                            Sentence[chatID][lineCount, 2] = $"{vText[0]}{GameManager.Instance.playerName}{vText[1]}";
                        }
                        else
                        {
                            Sentence[chatID][lineCount, 2] = $"{GameManager.Instance.playerName}{vText[0]}";
                        }
                        break;
                    }
                }
            }


            Sentence[chatID][lineCount, ++j] = "x";
            max[chatID]++;
            lineCount++;
        }
    }
    //bool isBracketOpen;
    //bool checkText;
    //int openBracketIndex;
    //int closeBracketIndex;
    //int bracketCount = 0;

    /// <summary>
    /// ��� ��� �� ����
    /// </summary>
    public void BgmPlay()
    {
        string bgmName = Sentence[chatID][lineNumber, (int)IDType.BGM];
        if (bgmName == "����")
            GameManager.Instance.BgmSound.StopSound();
        else if (bgmName != "")
        {
            GameManager.Instance.BgmSound.PlaySound(TextSO.bgmList[Convert.ToInt32(bgmName)], true);
        }
    }

    /// <summary>
    /// ���ȭ�� ����
    /// </summary>
    public void SetBackground()
    {
        if (Sentence[chatID][lineNumber, (int)IDType.BackgroundID] != "")
        {
            backgroundID = Convert.ToInt32(Sentence[chatID][lineNumber, (int)IDType.BackgroundID]) - 1;
            TextSO.backgroundList[backgroundID].gameObject.SetActive(true);
            //StartCoroutine(GameManager.Instance.FadeIn());
            StartCoroutine(GameManager.Instance.FadeOut());
        }
    }

    /// <summary>
    /// �̹��� �����ֱ�
    /// </summary>
    public void ShowImage()
    {
        if (Sentence[chatID][lineNumber, (int)IDType.ImageID] != "")
        {
            ImageSetActive(true);
        }
    }

    /// <summary>
    /// ��� Ÿ����
    /// </summary>
    public void Typing()
    {
        if(Sentence[chatID] == null)
        {
            StartCoroutine(LoadTextData(URL));
        }

        StartCoroutine(TypingCoroutine());
    }

    /// <summary>
    /// ��� Ÿ���� �ڷ�ƾ �Լ�
    /// </summary>
    /// <returns></returns>
    public IEnumerator TypingCoroutine()
    {
        textPanelObj.SetActive(true);
        if (!isAuto) _endAnimationObj.SetActive(false);

        isTyping = true;

        string pName = Sentence[chatID][lineNumber, (int)IDType.CharacterName];
        string storyText = Sentence[chatID][lineNumber, (int)IDType.Text];

        for (int i = 0; i < storyText.Length + 1; i++)
        {
            if (skip)
            {
                _textPanel.text = string.Format("{0}\n{1}", pName, storyText);           // �ؽ�Ʈ �ѱ�.....������ ������ �ѹ��� ��
                skip = false;
                break;
            }

//<<<<<<< HEAD
           // _textPanel.text = string.Format("{0}\n{1}", pName, Sentence[chatID][typingID, 2].Substring(0, i));
//=======
            
            _textPanel.text = string.Format("{0}\n{1}", pName, storyText.Substring(0, i));
//>>>>>>> 7e46c043bf5cc88230384ea197e1de2913820438
            //soundManager.TypingSound(); // �ؽ�Ʈ ���....��������
            yield return new WaitForSeconds(chatSpeed);
        }

        ///���� Ÿ���� ���� �� ����

        if (!isAuto) _endAnimationObj.SetActive(true);
        isTyping = false;

        GameObject tl = Instantiate(textlogPrefab, textlogView);
        if (pName == "") tl.GetComponent<Text>().text = storyText;
        else tl.GetComponent<Text>().text = string.Format("{0}: {1}", pName, storyText);
        Instantiate(textlogPrefab, textlogView);

        if (isAuto)
        {
            yield return new WaitForSeconds(autoSpeed);
            SkipText();
        }
    }

    public void SkipTextClick() // �ؽ�Ʈ �г� Ŭ�� �� ��ŵ
    {
        if (!isAuto)
        {
            if (!isTyping) SkipText();

            else
            {
                skip = true;
                EffectObject.SkipDotweenAnimation = true;
            }
        }
    }

    public void SkipText() // �ؽ�Ʈ ����
    {
        if (backgroundID >= 1) TextSO.backgroundList[backgroundID].gameObject.SetActive(true);

        if (Sentence[chatID][lineNumber, (int)IDType.ImageID] != "" && Sentence[chatID][lineNumber,(int)IDType.ImageID] != null) 
            ImageSetActive(false);

        if(Sentence[chatID][lineNumber, (int)IDType.Event] != "" && Sentence[chatID][lineNumber, (int)IDType.Event] != null)
        {
            string eventName = Sentence[chatID][lineNumber, (int)IDType.Event];
            if (eventName == "�Լ�")
            {
                string funcName = Sentence[chatID][lineNumber, (int)IDType.Event + 1];
                StartCoroutine(funcName.Trim());
            }
            else if (eventName == "�̵�")
            {
                int num = UnityEngine.Random.Range((int)IDType.Event + 1, Convert.ToInt32(Sentence[chatID][lineNumber, 19]));
                chatID = Convert.ToInt32(Sentence[chatID][lineNumber, num]);
                lineNumber = 0;
            }
            
            if (eventName == "����")
            {
                if (Sentence[chatID][lineNumber, (int)IDType.BackgroundID] != "")
                {
                    backgroundID = Convert.ToInt32(Sentence[chatID][lineNumber, (int)IDType.BackgroundID]) - 1;
                    TextSO.backgroundList[backgroundID].SetActive(true);
                }
                textPanelObj.SetActive(false);
                SelectOpen();
                return;
            }
        }

        lineNumber++;
        if (lineNumber != max[chatID])
        {
            TextTyping?.Invoke();
        }
        else textPanelObj.SetActive(false);
    } 

    public void AutoPlay()
    {
        isAuto = !isAuto;
        if (!isTyping)
        {
            SkipText();
        }
 
        _endAnimationObj.SetActive(false);
    }

    public void SelectOpen()
    {
        for (int i = 6; i < Convert.ToInt32(Sentence[chatID][lineNumber, 19]); i++)
        {
            select.Add(Sentence[chatID][lineNumber, i]);
            GameObject button = Instantiate(_selectButton, selectPanel);
            Text selectText = button.transform.GetChild(0).GetComponent<Text>();
            selectText.text = Sentence[chatID][lineNumber, i];
            select.Add(Sentence[chatID][lineNumber, ++i]);
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
                _textPanel.gameObject.SetActive(true);
                
                lineNumber = 1;
                TextTyping?.Invoke();
            }
        }
    }

    private void ImageSetActive(bool set)
    {
        string[] imageList = Sentence[chatID][lineNumber, (int)IDType.ImageID].Split(',');
        foreach (string x in imageList)
        {
            int num = Convert.ToInt32(x) - 1;

            TextSO.imageList[num].gameObject.SetActive(set);
        }
    }

    public void CallCameraShake()
    {
        //Camera.main.GetComponent<CameraShaking>().ShakeCam(shakeTime);
        CameraShaking.Instance.ShakeCam(shakeTime, shakestr);
    }

    public void InputNameCanvasOpen()
    {
        GameManager.Instance.InputNameCanvas.SetActive(true);
    }

    private void TestFunction()
    {
        print("�����");
    }

    IEnumerator GotoFreeTime()
    {
        SceneManager.LoadScene("FreeTime");
        yield return null;
    }
}

