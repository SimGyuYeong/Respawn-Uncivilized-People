using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "SO/Text/TextDataSO")]
public class TextDataSO : ScriptableObject
{
    public Image[] backgroundList;
    public Image[] imageList;
    public AudioClip[] sfxList;
    public AudioClip[] bgmList;
}
