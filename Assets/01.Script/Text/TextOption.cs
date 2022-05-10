using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class TextOption : MonoBehaviour
{
    [SerializeField] private GameObject _textLogPanel;

    private float _saveChatSpeed = 0;

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
            _saveChatSpeed = TextManager.Instance.chatSpeed;
            TextManager.Instance.chatSpeed = 0.01f;
        }
        else if (TextManager.Instance.chatSpeed == 0.01f)
        {
            TextManager.Instance.chatSpeed = _saveChatSpeed;
        }
    }
}
