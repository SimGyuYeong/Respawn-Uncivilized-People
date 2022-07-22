using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Stage")]
public class StageSO : ScriptableObject
{
    public int stage;
    public bool[] isWallList = new bool[64];
    public List<PlayerData> playerDataList = new List<PlayerData>();
    public List<AIData> aiDataList = new List<AIData>();
}
