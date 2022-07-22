using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSpeedSetting : MonoBehaviour, IValueSetting
{
    public void ValueSetting(float value)
    {
        GameManager.Instance.TEXT.chatSpeed = value / 10;
        PlayerPrefs.SetFloat("chatSpeed", value);
    }
}
