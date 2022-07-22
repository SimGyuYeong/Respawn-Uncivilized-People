using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterVolumeSetting : MonoBehaviour, IValueSetting
{
    public AudioSource _source;
    public void ValueSetting(float value)
    {
        _source.volume = value;
        PlayerPrefs.SetFloat("BGM", value);
    }
}
