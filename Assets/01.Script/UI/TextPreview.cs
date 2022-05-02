using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextPreview : MonoBehaviour
{
    private TextManager textManager;
    [SerializeField]
    private Text previewText;
    private string previewPath = "�ؽ�Ʈ�� �̼ӵ��� ��µ˴ϴ�.";
    private bool previewVisible = false;

    private void Start()
    {
        textManager = FindObjectOfType<TextManager>().GetComponent<TextManager>();

        
    }

    void Update()
    {
        if(previewVisible == false)
        {
            StartCoroutine(Chasdfasd());
        }
    }
    IEnumerator Chasdfasd()
    {
        previewVisible = true;
        for (int i = 0; i < 15 + 1; i++)
        {
            previewText.text = string.Format(previewPath.Substring(0, i));
            yield return new WaitForSeconds(textManager.chatSpeed);
        }
        previewVisible = false;
    }
}
