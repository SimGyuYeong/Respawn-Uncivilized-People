using UnityEngine;

public class TileInform
{
    public TileInform(int _num, int _x, int _y, bool _wall = false, bool _enemy = false)
    {
        tileNum = _num;
        isWall = _wall;
        isEnemy = _enemy;
        Position = new Vector2Int(_x, _y);
    }

    public int tileNum;
    public bool isWall, isEnemy;
    public Vector2Int Position;
}
