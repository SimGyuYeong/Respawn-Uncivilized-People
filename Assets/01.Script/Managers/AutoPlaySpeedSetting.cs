using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlaySpeedSetting : MonoBehaviour
{
    public void SetMusicVolume(float volume)
    {
        GameManager.Instance.TEXT.autoSpeed = volume;
        PlayerPrefs.SetFloat("auto", volume);
    }
}
