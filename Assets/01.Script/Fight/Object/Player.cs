using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class PlayerData
{
    public string name;
    [Range(0, 100)]
    public int durabilityPoint;
    [Range(0, 100)]
    public int kineticPoint;
    public Vector2Int position;
    public string info;
}

public class Player : MonoBehaviour
{
    public string playerName;

    private int _maxDurabilityPoint = 0;
    public int MaxDurabilityPoint => _maxDurabilityPoint;
    private int _durabilityPoint = 100;
    public int DurabilityPoint
    {
        get => _durabilityPoint;
        set
        {
            _durabilityPoint = value;
            if (_durabilityPoint > _maxDurabilityPoint) _durabilityPoint = _maxDurabilityPoint;
            transform.GetComponentInChildren<TextMeshProUGUI>().text = _durabilityPoint.ToString();
            
            if (_durabilityPoint <= 0)
            {
                FightManager.Instance.playerList.Remove(this);
                Destroy(gameObject);
            }
        }
    }

    private int _maxKineticPoint = 0;
    public int MaxKineticPoint => _maxKineticPoint;
    private int _kineticPoint = 100;
    public int KineticPoint
    {
        get => _kineticPoint;
        set
        {
            _kineticPoint = value;
            if (_kineticPoint > _maxKineticPoint) _kineticPoint = _maxKineticPoint;
            if (_kineticPoint < 0) _kineticPoint = 0;
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

    public void Init(int _id, PlayerData pData)
    {
        id = _id;
        playerName = pData.name;
        Position = pData.position;
        info = pData.info;
        _maxDurabilityPoint = pData.durabilityPoint;
        _durabilityPoint = pData.durabilityPoint;
        _maxKineticPoint = pData.kineticPoint;
        _kineticPoint = pData.kineticPoint;
    }
}
