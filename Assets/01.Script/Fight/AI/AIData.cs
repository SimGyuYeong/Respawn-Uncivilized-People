using UnityEngine;

[System.Serializable]
public class AIData
{
    public string name;
    [Range(0, 100)]
    public int influencePoint;
    public Vector2Int position;
    public int range;
    public string info;
    public AIType type;

    public enum AIType
    {
        LatitudeAI = 0,
        EfficientAI = 1
    }
}
