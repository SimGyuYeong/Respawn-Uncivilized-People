using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.Events;

public class TextManager : MonoBehaviour
{
    const string URL = "https://docs.google.com/spreadsheets/d/18d1eO7_f3gewvcBi5MIe0sqh50lp1PF-kkQg2nm03wg/export?format=tsv";

    private TextDataSave _textDataSO;
    public TextDataSave TextSO => _textDataSO;

    private Text _textPanel;
    public GameObject textPanelObj;

    [SerializeField] private Transform selectPanel; 

    [SerializeField] private GameObject _selectButton;
    public GameObject[] background; // 게임 배경화면 배열
    public GameObject[] image;    // 추가로 띄워둘 이미지 
    [SerializeField] private GameObject _endAnimationObj; // 텍스트 끝나면 텍스트 패널 오른쪽 아래에서 뛰옹뛰옹 뱅글뱅글 하는 애
    [SerializeField] private float shakeTime = 0.13f;
    [SerializeField] private float shakestr = 0;
    [SerializeField] private GameObject textlogPrefab;
    [SerializeField] private Transform textlogView;
    [SerializeField] private Text autoChecker;
    
    Dictionary<int, string[,]> Sentence = new Dictionary<int, string[,]>();
    Dictionary<int, int> max = new Dictionary<int, int>();
    List<string> select = new List<string>();

    public int chatID = 1, typingID = 1, backgroundID = 1, slotID = 0; // ID 기본 값 설정
    private bool isTyping = false, skip = false; // 텍스트 스킵 불 기본 값 설정
    public float chatSpeed = 0.1f, autoSpeed = 1f; // 텍스트 나오는 속도와 Auto 속도 기본 값 설정
    public bool isAuto = false;

    private static TextManager instance;

    public Action<GameObject> OnEffectObject;
    public UnityEvent TextTyping;
    public UnityEvent OnTextTypingEnd;

    enum IDType
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
            autoChecker.text = "<color=red>자동진행</color>";
        }
        else
        {
            autoChecker.text = "자동진행";
        }
    }

    private void Awake()
    {
        _textDataSO = GetComponentInChildren<TextDataSave>();
        StartCoroutine(LoadTextData()); // 텍스트 데이터 읽기
        _textPanel = textPanelObj.transform.Find("text").GetComponent<Text>();
    }

    public IEnumerator LoadTextData()
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
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
                Sentence[chatID][lineCount, j] = row[j];
                if(j > 4)
                {
                    if (row[j] == "") break;
                }
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
    /// 브금 재생 및 정지
    /// </summary>
    public void BgmPlay()
    {
        string bgmName = Sentence[chatID][typingID, (int)IDType.BGM];
        if (bgmName == "끄기")
            GameManager.Instance.BgmSound.StopSound();
        else if (bgmName != "")
        {
            GameManager.Instance.BgmSound.PlaySound(TextSO.bgmList[Convert.ToInt32(bgmName)], true);
        }
    }

    /// <summary>
    /// 배경화면 설정
    /// </summary>
    public void SetBackground()
    {
        if (Sentence[chatID][typingID, (int)IDType.BackgroundID] != "")
        {
            backgroundID = Convert.ToInt32(Sentence[chatID][typingID, (int)IDType.BackgroundID]) - 1;
            TextSO.backgroundList[backgroundID].gameObject.SetActive(true);
            //StartCoroutine(GameManager.Instance.FadeIn());
            StartCoroutine(GameManager.Instance.FadeOut());
        }
    }

    /// <summary>
    /// 이미지 보여주기
    /// </summary>
    public void ShowImage()
    {
        if (Sentence[chatID][typingID, (int)IDType.ImageID] != "")
        {
            ImageSetActive(true);
        }
    }

    /// <summary>
    /// 대사 타이핑
    /// </summary>
    public void Typing()
    {
        StartCoroutine(TypingCoroutine());
    }

    /// <summary>
    /// 대사 타이핑 코루틴 함수
    /// </summary>
    /// <returns></returns>
    public IEnumerator TypingCoroutine()
    {
        textPanelObj.SetActive(true);
        if (!isAuto) _endAnimationObj.SetActive(false);

        isTyping = true;
        
        string pName = Sentence[chatID][typingID, (int)IDType.CharacterName];
        if (pName == "당신") pName = GameManager.Instance.playerName;

        for (int i = 0; i < Sentence[chatID][typingID, (int)IDType.Text].Length + 1; i++)
        {
            if (skip)
            {
                _textPanel.text = string.Format("{0}\n{1}", pName, Sentence[chatID][typingID, 2]);           // 텍스트 넘김.....누르면 한줄이 한번에 딱
                skip = false;
                break;
            }


            //if(isBracketOpen)
            //{
            //    if (Sentence[chatID][typingID, 2][i] == '>')
            //    {
            //        checkText = true;
            //        isBracketOpen = false;
            //    }
            //    textPanel.text = string.Format("{0}\n{1}", Name, Sentence[chatID][typingID, 2].Substring(0, i));
            //    continue;
            //}
            //if (Sentence[chatID][typingID, 2][i] == '<') isBracketOpen = true;


            _textPanel.text = string.Format("{0}\n{1}", pName, Sentence[chatID][typingID, 2].Substring(0, i));
            //soundManager.TypingSound(); // 텍스트 출력....따따따따
            yield return new WaitForSeconds(chatSpeed);
        }

        ///↓↓↓ 타이핑 끝난 후 실행

        if (!isAuto) _endAnimationObj.SetActive(true);
        isTyping = false;

        GameObject tl = Instantiate(textlogPrefab, textlogView);
        if (pName == "") tl.GetComponent<Text>().text = string.Format(Sentence[chatID][typingID, 2]);
        else tl.GetComponent<Text>().text = string.Format("{0}: {1}", pName, Sentence[chatID][typingID, 2]);
        Instantiate(textlogPrefab, textlogView);

        if (isAuto)
        {
            yield return new WaitForSeconds(autoSpeed);
            SkipText();
        }
    }

    public void SkipTextClick() // 텍스트 패널 클릭 시 스킵
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

    public void SkipText() // 텍스트 진행
    {
        if (backgroundID >= 1) TextSO.backgroundList[backgroundID].gameObject.SetActive(true);
        if (Sentence[chatID][typingID, 4] != "") ImageSetActive(false);

        string eventName = Sentence[chatID][typingID, (int)IDType.Event];

        if (eventName == "함수") Invoke(Sentence[chatID][typingID, (int)IDType.Event + 1], 0f);
        else if (eventName == "이동")
        {
            int num = UnityEngine.Random.Range((int)IDType.Event + 1, Convert.ToInt32(Sentence[chatID][typingID, 19]));
            chatID = Convert.ToInt32(Sentence[chatID][typingID, num]);
            typingID = 0;
        }
        if (eventName == "선택")
        {
            if (Sentence[chatID][typingID, (int)IDType.BackgroundID] != "")
            {
                backgroundID = Convert.ToInt32(Sentence[chatID][typingID, (int)IDType.BackgroundID]) - 1;
                TextSO.backgroundList[backgroundID].SetActive(true);
            }
            textPanelObj.SetActive(false);
            SelectOpen();
        }
        else
        {
            typingID++;
            if (typingID != max[chatID])
            {
                TextTyping?.Invoke();
            }
            else textPanelObj.SetActive(false);
        }
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
        for (int i = 6; i < Convert.ToInt32(Sentence[chatID][typingID, 19]); i++)
        {
            select.Add(Sentence[chatID][typingID, i]);
            GameObject button = Instantiate(_selectButton, selectPanel);
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
                _textPanel.gameObject.SetActive(true);
                
                typingID = 1;
                TextTyping?.Invoke();
            }
        }
    }

    private void ImageSetActive(bool set)
    {
        string[] imageList = Sentence[chatID][typingID, (int)IDType.ImageID].Split(',');
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
        print("따라란");
    }
}

