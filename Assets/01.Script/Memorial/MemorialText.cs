using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MemorialText
{
    public string text;
    public bool isUnlock = false;

    public MemorialText(MemorialText _text)
    {
        this.text = _text.text;
        this.isUnlock = _text.isUnlock;
    }
}
