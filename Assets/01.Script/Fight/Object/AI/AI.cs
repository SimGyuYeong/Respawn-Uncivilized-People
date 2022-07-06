using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System;

public class AI : MonoBehaviour
{
    [SerializeField] protected int _id;
    [SerializeField] protected string _name;
    [SerializeField] protected int _influencePoint;
    [SerializeField] protected Vector2 _pos;
    [SerializeField] protected string _info;
    private int _range;

    private int _maxInfluencePoint;
    public int MaxInfluencePoint => _maxInfluencePoint;

    public int InfluencePoint
    {
        get => _influencePoint;
        set
        {
            _influencePoint = value;
            
            if (_influencePoint > _maxInfluencePoint) _influencePoint = _maxInfluencePoint;

            _influenceText.text = _influencePoint.ToString();

            if (_influencePoint <= 0)
            {
                if(_isRestructuring == true) Death();
                else
                {
                    _influencePoint = 0;
                    _isRestructuring = true;
                    _influenceText.text = "R";
                }
            }
        }
    }

    public Vector2 Position { get => _pos; set => _pos = value; }
    public string Info => _info;
    public string Name => _name;

    protected GameObject _attackObj; //공격할 오브젝
    protected Player _attackPlayer; //공격할 플레이어

    private bool _isRestructuring = false; //제압되어있나?
    private bool _isDead = false; //죽었나?

    private TextMeshProUGUI _influenceText;

    protected enum AI_STATE
    {
        WAIT_L,
        WAIT_S,
        WAIT_R
    }
    //AI 상태 초기설정
    protected AI_STATE _state = AI_STATE.WAIT_L;

    /// <summary>
    /// 기본 데이터 설정
    /// </summary>
    /// <param name="_id"></param>
    /// <param name="aData"></param>
    public void Init(int _id, AIData aData)
    {
        this._id = _id;
        _name = aData.name;
        Position = aData.position;
        _info = aData.info;
        _maxInfluencePoint = aData.influencePoint;
        InfluencePoint = aData.influencePoint;
        _range = aData.range;
    }

    private void Awake()
    {
        _influenceText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Death()
    {
        FightManager.Instance.aiList.Remove(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// AI 턴 시작 함수
    /// </summary>
    public void AIMoveStart()
    {
        if(_isRestructuring == false)
        {
            TargetOfAttackCheck();
            StartCoroutine(AIMoveState());
        }else
        {
            if(_isDead == true)
            {
                _isDead = false;
                _isRestructuring = false;
                InfluencePoint = _maxInfluencePoint;
            }
            else  _isDead = true;
            FightManager.Instance.TurnChange();
        }
    }

    /// <summary>
    /// AI 움직이는 함수
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator AIMoveState()
    {
        _attackPlayer = _attackObj.GetComponent<Player>();
        int _stamina = 2;

        if (_attackObj != gameObject) //공격대상이 본인 오브젝트가 아니라면 (공격대상이 지정됬다면)
        {
            while (true)
            {
                if (AttackDistanceCheck())
                {
                    _attackPlayer.DurabilityPoint -= InfluencePoint;
                    break;
                }

                if (_stamina == 0) break;

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
    protected void ObjMove(int value)
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
    protected bool AttackDistanceCheck()
    { 
        float distance = Vector2.Distance(_pos, _attackPlayer.Position);
        if (distance <= 1)
            return true;

        return false;
    }

    protected void TargetOfAttackCheck()
    {
        _attackObj = gameObject;

        float distance = 0;
        distance = Vector2.Distance(_pos, FightManager.Instance.playerList[0].Position);
        if (distance <= _range)
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

    public bool IsRestructuring()
    {
        return _isRestructuring;
    }
}
