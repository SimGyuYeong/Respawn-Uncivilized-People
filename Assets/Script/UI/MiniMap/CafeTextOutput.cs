using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CafeTextOutput : MonoBehaviour
{
    public CafeTextInput texts;
    [SerializeField] private Text outputText;
    string[] text;

    void TextLoad()
    {
        for (int i = 0; i < texts.CafeAlbaText.Length; i++)
        {
            text[i] = texts.CafeAlbaText[i];
        }
        for (int i = 0; i < texts.MyText.Length; i++)
        {
            text[i] = texts.MyText[1];
        }
    }

    void Update()
    {
        if (CafeStore.textbool == true)
        {
            outputText.text = string.Format("{0}\n", texts);
        }
    }
}
