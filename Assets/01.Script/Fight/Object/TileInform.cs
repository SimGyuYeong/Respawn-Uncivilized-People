using UnityEngine;

public class TileInform
{
    public TileInform(int _num, int _x, int _y, bool _wall = false)
    {
        tileNum = _num;
        isWall = _wall;
        Position = new Vector2Int(_x, _y);
    }

    public int tileNum;
    public bool isWall;
    public Vector2Int Position;
}
