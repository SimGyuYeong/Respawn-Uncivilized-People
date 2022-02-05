using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextManager textManager = null;
    public TextManager TEXT { get { return textManager; } }

    [SerializeField] private DataManager dataManager = null;
    public DataManager DATA {  get { return dataManager; } }

    [SerializeField] private SoundManager soundManager = null;
    public SoundManager SOUND {  get { return soundManager; } }

    [SerializeField] public GameObject Buttons;
    [SerializeField] public GameObject TitlePanel;
    [SerializeField] GameObject optionPanel;

    public string PlayerName = "����";

    //�ɼ�
    [SerializeField] Slider chatSpeedSlider;
    [SerializeField] Slider audoSpeedSlider;
    [SerializeField] Dropdown dropdown;
    List<Resolution> resolutions = new List<Resolution>();
    private int resolutionNum;
    FullScreenMode screenMode;

    //FadeIn
    [SerializeField] Image BlackImage;
    [SerializeField] GameObject BlackImageObject;

    //�̸��Է�
    public GameObject InputNameCanvas;
    [SerializeField] private InputField inputField;

    private void Awake()
    {
        TitlePanel.SetActive(true);
        Buttons.SetActive(true);
        Instance = this;
        textManager = GetComponent<TextManager>();
        dataManager = GetComponent<DataManager>();
        soundManager = GetComponent<SoundManager>();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        TEXT.chatSpeed = PlayerPrefs.GetFloat("chatSpeed", 1);
        //chatSpeedSlider.maxValue = 0.3f; // �����̴��� �ִ��� 0.3���� ����
        //chatSpeedSlider.minValue = 0.05f; // �����̴��� �ּڰ��� 0.05�� ����
        chatSpeedSlider.value = chatSpeedSlider.value * -1; // �����̴� �� -1 ���ϱ�
        chatSpeedSlider.value = TEXT.chatSpeed; // �����̴� ���� chatspeed�� ����
        TEXT.autoSpeed = PlayerPrefs.GetFloat("auto", 1);
        audoSpeedSlider.value = audoSpeedSlider.value * -1;
        audoSpeedSlider.value = TEXT.autoSpeed;
        Debug.Log(PlayerPrefs.GetFloat("auto", 1));
        soundManager.PlayingMusic(0, 0.01f);

        resolutions.AddRange(Screen.resolutions);
        dropdown.options.Clear();

        int optionNum = 0;
        foreach(Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + "x" + item.height + " " + item.refreshRate + "hz";
            dropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                dropdown.value = optionNum;
            optionNum++;
        }
        dropdown.RefreshShownValue();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            OptionPanelOC(0);
        }
    }

    public void Game(string name)
    {
        switch (name)
        {
            case "����":
                TitlePanel.SetActive(false);
                Buttons.SetActive(false);
                textManager.chatID = 1;
                soundManager.PauseMusic();
                //soundManager.PlayingMusic(1, 0.01f);
                StartCoroutine(FadeIn());
                StartCoroutine(textManager.Typing());
                break;
            case "����":
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
            default:
                break;
        }
    }

    public void OptionPanelOC(int check)
    {
        if (check == 1)
        {
            optionPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        else optionPanel.SetActive(true);
    }

    public IEnumerator FadeIn()
    {
        Debug.Log("����");
        BlackImageObject.SetActive(true);
        Color color = BlackImage.color;
        color.a = 1f;
        while (color.a >= 0)
        {
            color.a -= 0.06f;
            BlackImage.color = color;
            yield return new WaitForSeconds(0.1f);
        }
        BlackImageObject.SetActive(false);
    }

    public IEnumerator FadeOut()
    {
        BlackImageObject.SetActive(true);
        Color color = BlackImage.color;
        while (color.a != 100)
        {
            color.a += 0.05f;
            BlackImage.color = color;
            BlackImageObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;
    }

    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height, screenMode);
    }

    public IEnumerator CameraShaking()
    {
        Camera.main.GetComponent<CameraShaking>().ShakeCam(0.13f, 1000);
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator StopCroutine()
    {
        StopAllCoroutines();
        yield break;
    }

    public void Fullscene(bool is_fullscene)
    {
        Screen.fullScreen = is_fullscene;
    }

    public void BackTitle()
    {
        StopAllCoroutines();
        OptionPanelOC(1);
        BlackImageObject.SetActive(false);
        TEXT.textImage.gameObject.SetActive(false);
        TEXT.background[TEXT.backID].gameObject.SetActive(false);
        TitlePanel.SetActive(true);
        Buttons.SetActive(true);
        soundManager.PauseMusic();
        soundManager.PlayingMusic(0, 0.01f);
    }

    public void InputName()
    {
        PlayerName = inputField.text;
        InputNameCanvas.SetActive(false);
        TEXT.chatID = 100003;
        StartCoroutine(TEXT.LoadTextData());
        StartCoroutine(TEXT.Typing());
    }
}
