using UnityEngine;

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
