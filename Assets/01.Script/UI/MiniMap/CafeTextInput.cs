using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cafe TextData", menuName = "Scriptable Object/Cafe TextData", order = 1)]
public class CafeTextInput : ScriptableObject
{
    [SerializeField]
    public string[] CharacterName;  //누가 대사를 치는 것인가..이름
    [SerializeField]
    public string[] CafeText;  //어디서 무슨 대사를 치는 것인가... 대사
    
}