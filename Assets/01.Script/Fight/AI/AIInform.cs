using UnityEngine;

public enum AI_STATE
{
    WAIT_L = 1,
    WAIT_R,
    WAIT_S
}

public class AIInform
{
    public AIInform(int _num, int _x, int _y, int _health, int _timeNum)
    {
        TileNum = _timeNum;
        Number = _num;
        Health = _health;
        Position = new Vector2Int(_x, _y);
    }

    public int Number, Health, TileNum;
    public Vector2Int Position;
}
