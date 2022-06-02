using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

[System.Serializable]
public class AIData
{
    public string name;
    [Range(0, 100)]
    public int influencePoint;
    public Vector2Int position;
    public string info;
}

public class AI : MonoBehaviour
{
    public string aiName;
    public int id;

    private int _maxInfluencePoint;
    public int MaxInfluencePoint => _maxInfluencePoint;
    private int _influence;
    public int InfluencePoint
    {
        get => _influence;
        set
        {
            _influence = value;
            
            if (_influence > _maxInfluencePoint) _influence = _maxInfluencePoint;
            if (_influence < 0) _influence = 0;
            
            transform.GetComponentInChildren<TextMeshProUGUI>().text = _influence.ToString();
        }
    }
    public string info;

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
        }
    }

    //공격대상 오브젝트
    private GameObject _attackObj;

    private Player _attackPlayer;

    //행동력
    private int _stamina = 2;

    public enum AI_STATE
    {
        WAIT_L,
        WAIT_S,
        WAIT_R
    }
    //AI 상태 초기설정
    private AI_STATE _state = AI_STATE.WAIT_L;

    public void Init(int _id, AIData aData)
    {
        id = _id;
        aiName = aData.name;
        Position = aData.position;
        info = aData.info;
        _maxInfluencePoint = aData.influencePoint;
        InfluencePoint = aData.influencePoint;
    }

    /// <summary>
    /// AI 턴 시작 함수
    /// </summary>
    public void AIMoveStart()
    {
        TargetOfAttackCheck();
        StartCoroutine(AIMove());
    }

    /// <summary>
    /// AI 움직이는 함수
    /// </summary>
    /// <returns></returns>
    private IEnumerator AIMove()
    {
        _attackPlayer = _attackObj.GetComponent<Player>();

        if (_attackObj != gameObject) //공격대상이 본인 오브젝트가 아니라면 (공격대상이 지정됬다면)
        {
            _stamina = 2; 
            while (_stamina > 0)
            {
                if (AttackDistanceCheck())
                {
                    _attackPlayer.DurabilityPoint -= InfluencePoint;
                    break;
                }

                if (_pos.x == _attackPlayer.Position.x)
                {
                    if (_pos.y - _attackPlayer.Position.y > 0)
                    {
                        if (!FightManager.Instance.ObjCheck(_pos, 'd'))
                            ObjMove(8);
                    }

                    else
                    {
                        if (!FightManager.Instance.ObjCheck(_pos, 'u'))
                            ObjMove(-8);
                    }

                }
                else
                {
                    if (_pos.x - _attackPlayer.Position.x > 0)
                    {
                        if (!FightManager.Instance.ObjCheck(_pos, 'l'))
                            ObjMove(-1);
                    }

                    else
                    {
                        if (!FightManager.Instance.ObjCheck(_pos, 'r'))
                            ObjMove(1);
                    }

                }

                _stamina--;
                yield return new WaitForSeconds(1f);
            }
        }

        else
        {

            if (FightManager.Instance.ObjCheck(_pos, 'r')
                && FightManager.Instance.ObjCheck(_pos, 'l'))
                {
                _state = AI_STATE.WAIT_S;
            }

            switch (_state)
            {
                case AI_STATE.WAIT_R:
                    ObjMove(1);
                    if (FightManager.Instance.ObjCheck(_pos, 'r'))
                        _state = AI_STATE.WAIT_L;

                    break;

                case AI_STATE.WAIT_L:
                    ObjMove(-1);
                    if (FightManager.Instance.ObjCheck(_pos, 'l'))
                        _state = AI_STATE.WAIT_R;
                        
                    break;

                case AI_STATE.WAIT_S:
                    if (!FightManager.Instance.ObjCheck(_pos, 'l'))
                    {
                        ObjMove(-1);
                        _state = AI_STATE.WAIT_L;
                    }

                    else if (!FightManager.Instance.ObjCheck(_pos, 'r'))
                    {
                        ObjMove(1);
                        _state = AI_STATE.WAIT_R;
                    }

                    break;
            }
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);
        FightManager.Instance.TurnChange();
    }

    /// <summary>
    /// Enemy 오브젝트 이동 함수
    /// </summary>
    /// <param name="value"></param>
    private void ObjMove(int value)
    {
        if (value == 1 || value == -1)
            _pos.x += value;
        else if (value == 8)
            _pos.y--;
        else
            _pos.y++;

        int slot = Mathf.FloorToInt((7 - _pos.y) * 8 + _pos.x);
        transform.SetParent(FightManager.Instance.tileList[slot].transform);

        transform.DOMove(new Vector3(_pos.x, _pos.y), 1F);
    }

    /// <summary>
    /// 공격대상 지정 함수
    /// </summary>
    /// <returns>공격대상 지정 여부</returns>
    private bool AttackDistanceCheck()
    { 
        float distance = Vector2.Distance(_pos, _attackObj.GetComponent<Player>().Position);
        if (distance <= 1)
            return true;

        return false;
    }

    private void TargetOfAttackCheck()
    {
        _attackObj = gameObject;

        float distance = 0;
        distance = Vector2.Distance(_pos, FightManager.Instance.playerList[0].Position);
        if (distance <= 3)
        {
            _attackObj = FightManager.Instance.playerList[0].gameObject;
        }

        foreach (var p in FightManager.Instance.playerList)
        {
            Vector2 pos = p.Position;

            float dis2 = Vector2.Distance(_pos, pos);
            if(dis2 < distance)
            {
                distance = dis2;
                _attackObj = p.gameObject;
            }
        }
    }
}
