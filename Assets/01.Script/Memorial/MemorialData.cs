using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/MemorialData")]
//[System.Serializable]
public class MemorialData : ScriptableObject
{
    public Sprite[] wordImage;

    public string[] wordMeaning;
}