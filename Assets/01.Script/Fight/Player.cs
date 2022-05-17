using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string DName;
    public int DEnergy;
    public Vector2 DPos;
    public string DInfo;
}

public class Player : MonoBehaviour
{
    public string playerName;

    private int _energy = 100;
    public int Energy
    {
        get => _energy;
        set
        {
            _energy = value;
            if (_energy > 100) _energy = 100;
            if (_energy < 0) _energy = 0;
        }
    }

    public bool isMove = false;
    public bool isFight = false;
    public int id = 0;

    private Vector2 _pos = Vector2.zero;
    public Vector2 Position
    {
        get
        {
            return _pos;
        }
        set
        {
            _pos = value;
            transform.position = value;
            FightManager.Instance.pPos = IPos;
        }
    }

    public Vector2Int IPos => new Vector2Int((int)_pos.x, (int)_pos.y);
    public string info;
}
