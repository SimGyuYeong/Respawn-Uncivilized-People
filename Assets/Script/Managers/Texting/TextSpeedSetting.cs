using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSpeedSetting : MonoBehaviour
{
    public void SetMusicVolume(float volume)
    {
        GameManager.Instance.TEXT.chatSpeed = volume / 10;
        PlayerPrefs.SetFloat("chatSpeed", volume);
    }
}
