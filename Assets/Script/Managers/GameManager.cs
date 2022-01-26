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


    //�ɼ�
    [SerializeField] Slider chatSpeedSlider;

    //FadeIn
    [SerializeField] Image BlackImage;
    [SerializeField] GameObject BlackImageObject;

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
        chatSpeedSlider.maxValue = 0.3f; // �����̴��� �ִ��� 0.3���� ����
        chatSpeedSlider.minValue = 0.05f; // �����̴��� �ּڰ��� 0.05�� ����
        TEXT.chatSpeed = 0.1f;
        chatSpeedSlider.value = chatSpeedSlider.value * -1; // �����̴� �� -1 ���ϱ�
        chatSpeedSlider.value = TEXT.chatSpeed; // �����̴� ���� chatspeed�� ����
        //soundManager.PlayingMusic(0, 0.01f);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            DATA.SaveMenuPanelOpen(1);
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
        if (check == 1) optionPanel.SetActive(false);
        else optionPanel.SetActive(true);
    }
    
    public void SetMusicVolume(float volume)
    {
        TEXT.chatSpeed = volume;
    }

    public IEnumerator FadeIn()
    {
        BlackImageObject.SetActive(true);
        Color color = BlackImage.color;
        while (color.a != 0)
        {
            color.a -= 0.05f;
            BlackImage.color = color;
            BlackImageObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
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
    public IEnumerator CameraShaking()
    {
        Camera.main.GetComponent<CameraShaking>().ShakeForTime(0.2f);
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator StopCroutine()
    {
        StopAllCoroutines();
        yield break;
    }
}
