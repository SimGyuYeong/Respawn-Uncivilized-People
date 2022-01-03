using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public void OptionPanelOC(int check)
    {
        if (check == 1)
        {
            optionPanel.SetActive(false);
        }
        else
        {
            optionPanel.SetActive(true);
        }
    }
}
