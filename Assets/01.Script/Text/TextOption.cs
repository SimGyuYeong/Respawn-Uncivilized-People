using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class TextOption : MonoBehaviour
{
    [SerializeField] private GameObject _textLogPanel;

    private float _defaultChatSpeed = 0;

    public DataManager data;

    private void Awake()
    {
        data = FindObjectOfType<DataManager>();
    }

    /// <summary>
    /// 대사록 보기
    /// </summary>
    /// <param name="check"></param>
    public void ShowTextLog(bool check)
    {
        _textLogPanel.SetActive(check);
    }

    /// <summary>
    /// 빨리감기
    /// </summary>
    public void FastSkipText()
    {
        if (TextManager.Instance.chatSpeed != 0.01f)
        {
            _defaultChatSpeed = TextManager.Instance.chatSpeed;
            TextManager.Instance.chatSpeed = 0.01f;
        }
        else if (TextManager.Instance.chatSpeed == 0.01f)
        {
            TextManager.Instance.chatSpeed = _defaultChatSpeed;
        }
    }

    public void SavePanelShow()
    {
        data.SaveMenuPanelOpen(1);
    }

    public void AutoSave()
    {
        data.SaveMenuPanelOpen(0);
    }

    public void LoadPanelShow()
    {
        data.SaveMenuPanelOpen(2);
    }

    public void AutoLoad()
    {
        data.SaveMenuPanelOpen(3);
    }
}
