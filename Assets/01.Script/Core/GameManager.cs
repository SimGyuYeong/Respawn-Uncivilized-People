using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    private TextManager _textManager = null;
    public TextManager TEXT { get { return _textManager; } }

    [SerializeField] private DataManager dataManager = null;
    public DataManager DATA {  get { return dataManager; } }

    [SerializeField] private SoundManager bgmSoundManager = null; 
    public SoundManager BgmSound {  get { return bgmSoundManager; } }

    [SerializeField] private SoundManager sfxSoundManager = null;
    public SoundManager SfxSound { get { return sfxSoundManager; } }

    [SerializeField] public GameObject Buttons;
    [SerializeField] public GameObject TitlePanel;
    [SerializeField] GameObject optionPanel;

    //옵션
    [SerializeField] Slider chatSpeedSlider;
    [SerializeField] Slider audoSpeedSlider;

    //FadeIn
    [SerializeField] Image BlackImage;
    [SerializeField] GameObject BlackImageObject;

    //크레딧 패널
    [SerializeField] private GameObject creditPanel;
    [SerializeField] private GameObject creditPanelCancelButton;

    private void Awake()
    {
        if (Instance != this)
        {
            Destroy(Instance);
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
        TitlePanel.SetActive(true);
        Buttons.SetActive(true);
        _textManager = transform.parent.Find("TextManager").GetComponent<TextManager>();
        dataManager = GetComponent<DataManager>();
        bgmSoundManager = transform.parent.Find("SoundManager").GetChild(0).GetComponent<SoundManager>();
        sfxSoundManager = transform.parent.Find("SoundManager").GetChild(1).GetComponent<SoundManager>();
    }

    private void Start()
    {
        TEXT.chatSpeed = PlayerPrefs.GetFloat("chatSpeed", 1);
        chatSpeedSlider.value = TEXT.chatSpeed; // 슬라이더 값을 chatspeed로 변경

        TEXT.autoSpeed = PlayerPrefs.GetFloat("auto", 1);
        audoSpeedSlider.value = audoSpeedSlider.value * -1;
        audoSpeedSlider.value = TEXT.autoSpeed;
        //bgmSoundManager.PlaySound(_textManager.TextSO.bgmList[0], true);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !TextUIManager.instance.isLoading)
        {
            if(DATA.savemenuPanel.activeSelf == true)
            {
                DATA.SaveMenuPanelClose();
            }
            else if(optionPanel.activeSelf == true && !TextUIManager.instance.isMemorial)
            {
                TextUIManager.instance.ShowOptionPanel(false);
            }
            else if(optionPanel.activeSelf == true && TextUIManager.instance.isMemorial)
            {
                TextUIManager.instance.DownMemorial();
            }
            else
            {
                Time.timeScale = 0;
                TextUIManager.instance.ShowOptionPanel(true);
            }
        }
    }

    public void Game(string name)
    {
        switch (name)
        {
            case "시작":
                TextUIManager.instance.Loading(() =>
                {
                    TitlePanel.SetActive(false);
                    Buttons.SetActive(false);
                    _textManager.chatID = 1;
                    //bgmSoundManager.PlaySound(_textManager.TextSO.bgmList[0], true);
                    StartCoroutine(FadeIn());
                    TEXT.TextTyping?.Invoke();
                });
                
                break;
            case "종료":
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
            case "크레딧":
                if (creditPanel.active.Equals(false))
                {
                    creditPanel.SetActive(true);
                }
                else
                {
                    creditPanel.SetActive(false);
                }
                break;
            default:
                break;
        }
    }

    public IEnumerator FadeIn()
    {
        Debug.Log("실행");
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

    public void BackTitle()
    {
        StopAllCoroutines();
        TextUIManager.instance.ShowOptionPanel(false);
        BlackImageObject.SetActive(false);
        TEXT.textPanelObj.SetActive(false); 
        TEXT.TextSO.backgroundList[TEXT.backgroundID].gameObject.SetActive(false);
        TitlePanel.SetActive(true);
        Buttons.SetActive(true);
        //bgmSoundManager.PlaySound(_textManager.TextSO.bgmList[0], true);
    }

    public IEnumerator CreditPanelScroll()
    {
        yield return new WaitForSeconds(1f);
    }
}
