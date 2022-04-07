using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInform : MonoBehaviour
{
    public AIInform(int _num, int _x, int _y, int _health)
    {
        Number = _num;
        Health = _health;
        Position = new Vector2(_x, _y);
    }

    public int Number, Health;
    public Vector2 Position;
}
