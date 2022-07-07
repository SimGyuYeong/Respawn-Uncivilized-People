using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextOption : MonoBehaviour
{
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color activeColor;

    private float _defaultChatSpeed = 0;

    private DataManager _dataManager;

    private void Awake()
    {
        _dataManager = FindObjectOfType<DataManager>();

        transform.Find("Log").GetComponent<PrefabButton>().OnClickEvent += ShowTextLog;
        transform.Find("Speed").GetComponent<PrefabButton>().OnClickEvent += FastSkipText;
        transform.Find("Auto").GetComponent<PrefabButton>().OnClickEvent += AutoPlay;
        transform.Find("Save").GetComponent<PrefabButton>().OnClickEvent += SavePanelShow;
        transform.Find("AutoSave").GetComponent<PrefabButton>().OnClickEvent += AutoSave;
        transform.Find("Load").GetComponent<PrefabButton>().OnClickEvent += LoadPanelShow;
        transform.Find("AutoLoad").GetComponent<PrefabButton>().OnClickEvent += AutoLoad;
        transform.Find("Option").GetComponent<PrefabButton>().OnClickEvent += ShowOptionPanel;
    }

    public void ShowTextLog()
    {
        TextUIManager.instance.textLogUI.SetActive(true);
    }

    /// <summary>
    /// »¡¸®°¨±â
    /// </summary>
    public void FastSkipText()
    {
        if (TextManager.Instance.chatSpeed != 0.01f)
        {
            _defaultChatSpeed = TextManager.Instance.chatSpeed;
            TextManager.Instance.chatSpeed = 0.01f;
            transform.Find("Speed").GetComponent<Text>().color = activeColor;
        }
        else if (TextManager.Instance.chatSpeed == 0.01f)
        {
            TextManager.Instance.chatSpeed = _defaultChatSpeed;
            transform.Find("Speed").GetComponent<Text>().color = defaultColor;
        }
    }

    public void SavePanelShow()
    {
        _dataManager.SaveMenuPanelOpen(1);
    }

    public void AutoSave()
    {
        _dataManager.SaveMenuPanelOpen(0);
    }

    public void LoadPanelShow()
    {
        _dataManager.SaveMenuPanelOpen(2);
    }

    public void AutoLoad()
    {
        _dataManager.SaveMenuPanelOpen(3);
    }

    public void ShowOptionPanel()
    {
        TextUIManager.instance.ShowOptionPanel(true);
    }

    public void AutoPlay()
    {
        TextManager.Instance.AutoPlay();
        if(TextManager.Instance.isAuto)
        {
            transform.Find("Auto").GetComponent<Text>().color = activeColor;
        }
        else
        {
            transform.Find("Auto").GetComponent<Text>().color = defaultColor;
        }
    }
}
