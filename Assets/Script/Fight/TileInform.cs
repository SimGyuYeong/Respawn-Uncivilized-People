using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInform : MonoBehaviour
{
    public TileInform(int _num, int _x, int _y, bool _wall)
    {
        tileNum = _num;
        x = _x;
        y = _y;
        isWall = _wall;
        Position = new Vector2Int(x, y);
    }

    public int tileNum, x, y;
    public bool isWall;
    public Vector2Int Position;
}
