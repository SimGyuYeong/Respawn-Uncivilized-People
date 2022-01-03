using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextManager textManager = null;
    public TextManager TEXT { get { return textManager; } }

    [SerializeField] private DataManager dataManager = null;
    public DataManager DATA {  get { return dataManager; } }

    [SerializeField] public GameObject TitlePanel;
    [SerializeField] GameObject optionPanel;

    private void Awake()
    {
        Instance = this;
        textManager = GetComponent<TextManager>();
        dataManager = GetComponent<DataManager>();
        DontDestroyOnLoad(gameObject);
        #region 사운드
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion
    }

    public void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void Game(string name)
    {
        switch (name)
        {
            case "시작":
                TitlePanel.SetActive(false);
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

<<<<<<< HEAD
    public void OptionPanelOC(int check)
    {
        if (check == 1) optionPanel.SetActive(false);
        else optionPanel.SetActive(true);
    }
=======
    public AudioSource bgsound;
    public AudioClip[] bglist;

    public static GameManager instance;

    #region 사운드

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for (int i = 0; i < bglist.Length; i++)
        {
            if (arg0.name == bglist[i].name)
                BgsoundPlay(bglist[i]);
        }
    }

    public void SFXPlay(string sfxName, AudioClip clip)
    {
        GameObject go = new GameObject(sfxName + "Sound");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = 0.3f;
        audioSource.Play();
        Destroy(go, clip.length);
    }

    public void BgsoundPlay(AudioClip clip)
    {
        bgsound.clip = clip;
        bgsound.loop = true;
        bgsound.volume = 0.1f;
        bgsound.Play();
    }
    #endregion
>>>>>>> Prototype
}
