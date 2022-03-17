using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cafe TextData", menuName = "Scriptable Object/Cafe TextData", order = 1)]
public class CafeTextInput : ScriptableObject
{
    [SerializeField]
    public string[] CafeAlbaText;
    [SerializeField]
    public string[] MyText;  
}