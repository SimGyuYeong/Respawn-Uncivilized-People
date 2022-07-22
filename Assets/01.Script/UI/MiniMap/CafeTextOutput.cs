using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CafeTextOutput : MonoBehaviour
{
    [SerializeField]
    private CafeTextInput texts;
    [SerializeField] private Text outputText;
    string[] text;
    public bool textSkip = true;

    private void Start()
    {

    }

    /// <summary>
    /// 텍스트 나오게 해주는 함수
    /// </summary>
    public void TextLoad(int charName, int backgroundtext)
    {
        for (int i = 0; i < texts.CafeText.Length; i++)
        {
            if(textSkip)
            {
                outputText.text = string.Format("{0}\n{1}", texts.CharacterName[charName], texts.CafeText[backgroundtext]);
            }
            text[i] = texts.CafeText[i];
        }
    }

    void Update()
    {
        if (StoreManager.textbool == true)
        {
            outputText.text = string.Format("{0}\n", texts);
        }
    }
}
