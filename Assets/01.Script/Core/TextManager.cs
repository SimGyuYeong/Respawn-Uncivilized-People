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

    protected TextMeshPro _textPanel;
    public GameObject textPanelObj;
    public GameObject textPanelPrefab;

    public Transform textCanvasTrm;

    [SerializeField] protected Transform selectPanel;

    protected Button textPanelBTN;

    [SerializeField] protected GameObject _selectButton;
    public GameObject[] background; // 게임 배경화면 배열
    public GameObject[] image;    // 추가로 띄워둘 이미지 
    protected GameObject _endAnimationObj; // 텍스트 끝나면 텍스트 패널 오른쪽 아래에서 뛰옹뛰옹 뱅글뱅글 하는 애
    [SerializeField] protected float shakeTime = 0.13f;
    [SerializeField] protected float shakestr = 0;
    [SerializeField] protected GameObject textlogPrefab;
    [SerializeField] protected Transform textlogView;
    [SerializeField] private Text autoChecker;
    
    protected Dictionary<int, string[,]> Sentence = new Dictionary<int, string[,]>();
    protected Dictionary<int, int> max = new Dictionary<int, int>();
    protected List<string> select = new List<string>();

    public int chatID = 1, lineNumber = 1, backgroundID = 0, slotID = 0; // ID 기본 값 설정
    protected bool isTyping = false, skip = false; // 텍스트 스킵 불 기본 값 설정
    public float chatSpeed = 0.1f, autoSpeed = 1f; // 텍스트 나오는 속도와 Auto 속도 기본 값 설정
    public bool isAuto = false;

    private static TextManager instance;

    public AudioClip _coffeclip = null;

    public Action<GameObject> OnEffectObject;
    public UnityEvent TextTyping;
    public UnityEvent OnTextTypingEnd;

    public SpriteRenderer _fadeImage;
    private bool _isEnd = false;

    protected int _time = 0;

    protected enum IDType
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
        //if(isAuto == true)
        //{
        //    autoChecker.text = "<color=red>자동진행</color>";
        //}
        //else
        //{
        //    autoChecker.text = "자동진행";
        //}
    }

    private void Awake()
    {
        _textDataSO = GetComponentInChildren<TextDataSave>();
        StartCoroutine(LoadTextData(URL)); // 텍스트 데이터 읽기
        
    }

    public void Start()
    {
        textPanelObj = Instantiate(textPanelPrefab, textCanvasTrm);

        _textPanel = textPanelObj.transform.Find("text").GetComponent<TextMeshPro>();
        textPanelBTN = textPanelObj.transform.Find("Button").GetComponent<Button>();
        _endAnimationObj = textPanelObj.transform.Find("textEnd").gameObject;

        textPanelBTN.onClick.AddListener(() => SkipTextClick());

        ChildStart();
    }

    protected virtual void ChildStart()
    {
        //do nothing!
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
    /// 브금 재생 및 정지
    /// </summary>
    public void BgmPlay()
    {
        string bgmName = Sentence[chatID][lineNumber, (int)IDType.BGM];
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
    public virtual void SetBackground()
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
    /// 이미지 보여주기
    /// </summary>
    public void ShowImage()
    {
        if (Sentence[chatID][lineNumber, (int)IDType.ImageID] != "")
        {
            ImageSetActive(true);
        }
    }

    /// <summary>
    /// 대사 타이핑
    /// </summary>
    public void Typing()
    {
        if(Sentence[chatID] == null)
        {
            StartCoroutine(LoadTextData(URL));
        }
        _isEnd = false;


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
        string pName = Sentence[chatID][lineNumber, (int)IDType.CharacterName];
        string storyText = Sentence[chatID][lineNumber, (int)IDType.Text];

        //if (Sentence[chatID][lineNumber, (int)IDType.Direct] == null || Sentence[chatID][lineNumber, (int)IDType.Direct] == "") {

        if (Sentence[chatID][lineNumber, (int)IDType.Direct] == "" || Sentence[chatID][lineNumber, (int)IDType.Direct] == null)
        {
            for (int i = 0; i < storyText.Length + 1; i++)
            {

                if (skip)
                {
                    _textPanel.text = string.Format("{0}\n{1}", pName, storyText);           // 텍스트 넘김.....누르면 한줄이 한번에 딱
                    skip = false;
                    break;
                }


                _textPanel.text = string.Format("{0}\n{1}", pName, storyText.Substring(0, i));
                //soundManager.TypingSound(); // 텍스트 출력....따따따따
                yield return new WaitForSeconds(chatSpeed);

                _textPanel.text = string.Format("{0}\n{1}", pName, storyText);
            }
        }

        else
        {
            _textPanel.text = string.Format("");
        }
        ///↓↓↓ 타이핑 끝난 후 실행

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

        //else
        //{
        //    SkipText();
        //}

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

        if (Sentence[chatID][lineNumber, (int)IDType.ImageID] != "" && Sentence[chatID][lineNumber,(int)IDType.ImageID] != null) 
            ImageSetActive(false);

        if (Sentence[chatID][lineNumber+1, (int)IDType.Direct] != "" && Sentence[chatID][lineNumber+1, (int)IDType.Direct] != null)
        {
            string directName = Sentence[chatID][lineNumber+1, (int)IDType.Direct];
            StartCoroutine(directName.Trim());
            //Invoke(directName.Trim(), 0f);
            Debug.Log(directName.Trim());
        }

        if (Sentence[chatID][lineNumber, (int)IDType.SFX] != "" && Sentence[chatID][lineNumber, (int)IDType.SFX] != null)
        {
            string clipName = Sentence[chatID][lineNumber, (int)IDType.SFX].Trim();
            //FreeTimeSFX.instance.PlaySoundClip(_coffeclip);
            Debug.Log(_coffeclip);
            FreeTimeSFX.instance.SfxSound.PlaySound(_coffeclip);
        }

        if (Sentence[chatID][lineNumber, (int)IDType.Event] != "" && Sentence[chatID][lineNumber, (int)IDType.Event] != null)
        {
            string eventName = Sentence[chatID][lineNumber, (int)IDType.Event];
            if (eventName == "함수")
            {
                string funcName = Sentence[chatID][lineNumber, (int)IDType.Event + 1];//StartCoroutine(funcName.Trim());
                //Debug.Log(funcName.Trim());
                StartCoroutine(funcName.Trim());
                if (_isEnd)
                {
                    return;
                }
            }
            else if (eventName == "이동")
            {
                int num = UnityEngine.Random.Range((int)IDType.Event + 1, Convert.ToInt32(Sentence[chatID][lineNumber, 19]));
                chatID = Convert.ToInt32(Sentence[chatID][lineNumber, num]);
                lineNumber = 1;
            }

            if (eventName == "선택")
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

    IEnumerator Walking()
    {
        Debug.Log("ascasda");
        textPanelBTN.interactable = false;
        //_textPanel.text = string.Format("");
        //FreeTimeDirect.Instance.FadeIn();
        FreeTimeDirect.Instance.Walking();
        
        textPanelObj.SetActive(false);
        yield return new WaitForSeconds(3f);
        textPanelObj.SetActive(true);
        FreeTimeDirect.Instance.FadeOutTextPanel();
        textPanelBTN.interactable = true;
        SkipText();
    }

    IEnumerator TakeALook()
    {
        textPanelBTN.interactable = false;
        FreeTimeDirect.Instance.LookAround(() =>
        {
            SkipText();
            textPanelBTN.interactable = true;
        });
        
        yield return new WaitForSeconds(3f);
    }

    IEnumerator FadeOut()
    {
        if (_isEnd == false)
        {
            textPanelBTN.interactable = false;
            FreeTimeDirect.Instance.FadeOut();
            yield return new WaitForSeconds(1.7f);
            textPanelBTN.interactable = true;
            SkipText();
        }

        else
        {
            FreeTimeDirect.Instance.FadeOut();
            yield return new WaitForSeconds(1.7f);
        }
    }

    IEnumerator FadeIn()
    {
        if (_isEnd == false)
        {
            textPanelBTN.interactable = false;
            FreeTimeDirect.Instance.FadeIn();
            yield return new WaitForSeconds(1.7f);
            textPanelBTN.interactable = true;
            SkipText();
        }

        else
        {
            FreeTimeDirect.Instance.FadeIn();
            yield return new WaitForSeconds(1.7f);
        }
    }

    IEnumerator GoToMain()
    {
        _time++;
        _isEnd = true;
        textPanelBTN.interactable = false;
        FreeTimeDirect.Instance.FadeOutTextPanel();
        textPanelObj.gameObject.SetActive(false);
        StartCoroutine(FadeOut());
        yield return new WaitForSeconds(1f);
        transform.GetComponentInChildren<FreeTimeText>().GoToMainScreen();
        yield return new WaitForSeconds(1f);
        StartCoroutine(FadeIn());
        transform.GetComponentInChildren<FreeTimeText>().SetButton();
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
        print("따라란");
    }

    IEnumerator GotoFreeTime()
    {
        SceneManager.LoadScene("FreeTime");
        yield return null;
    }
}