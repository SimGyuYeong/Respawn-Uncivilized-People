using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextManager textManager = null;
    public TextManager TEXT { get { return textManager; } }

    [SerializeField] private DataManager dataManager = null;
    public DataManager DATA {  get { return dataManager; } }

    [SerializeField] GameObject TitlePanel;

    private void Awake()
    {
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
            case "����":
                TitlePanel.SetActive(false);
                textManager.chatID = 1;
                StartCoroutine(textManager.Typing());
                break;
            default:
                break;
        }
    }
}
