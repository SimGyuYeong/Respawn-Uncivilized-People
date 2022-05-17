using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void DataSet(int _id, ObjData pData)
    {
        id = _id;
        playerName = pData.DName;
        Position = pData.DPos;
        info = pData.DInfo;
        _energy = pData.DEnergy;
    }
}
