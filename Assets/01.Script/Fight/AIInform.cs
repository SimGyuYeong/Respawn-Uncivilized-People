using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInform : MonoBehaviour
{
    public AIInform(int _num, int _x, int _y, int _health)
    {
        Number = _num;
        x = _x;
        y = _y;
        Health = _health;
        Position = new Vector2(x, y);
    }

    int Number, x, y, Health;
    Vector2 Position;
}
