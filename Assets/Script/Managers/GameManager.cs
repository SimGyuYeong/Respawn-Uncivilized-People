using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextManager textManager = null;
    public TextManager TEXT { get { return textManager; } }

    [SerializeField] private DataManager dataManager = null;
    public DataManager DATA {  get { return dataManager; } }

    [SerializeField] public GameObject Buttons;
    [SerializeField] public GameObject TitlePanel;
    [SerializeField] GameObject optionPanel;

    [SerializeField] public Camera mainCamera;
    [SerializeField] public Camera shakingCamera;

    //옵션
    [SerializeField] Slider chatSpeedSlider;

    private void Awake()
    {
        TitlePanel.SetActive(true);
        Buttons.SetActive(true);
        Instance = this;
        textManager = GetComponent<TextManager>();
        dataManager = GetComponent<DataManager>();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        chatSpeedSlider.maxValue = 0.3f; // 슬라이더의 최댓값을 0.3으로 설정
        chatSpeedSlider.minValue = 0.05f; // 슬라이더의 최솟값을 0.05로 설정
        TEXT.chatSpeed = 0.1f;
        chatSpeedSlider.value = TEXT.chatSpeed; // 슬라이더 값을 chatspeed로 변경

        shakingCamera.gameObject.SetActive(false);
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
            case "시작":
                TitlePanel.SetActive(false);
                Buttons.SetActive(false);
                textManager.chatID = 1;
                StartCoroutine(textManager.Typing());
                break;
            case "종료":
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

    public void CameraChange(string cN)
    {
        if (cN == "main")
        {
            mainCamera.gameObject.SetActive(true);
            shakingCamera.gameObject.SetActive(false);
        }
        else if(cN == "Shake")
        {
            mainCamera.gameObject.SetActive(false);
            shakingCamera.gameObject.SetActive(true);
            CameraShaking.Instance.OnShakeCamera();
        }
    }
}
