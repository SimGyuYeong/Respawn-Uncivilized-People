using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class AI : MonoBehaviour
{
    public string aiName;
    public int id;

    private int _maxEnergy;
    private int _energy;
    public int Energy
    {
        get => _energy;
        set
        {
            _energy = value;
            
            if (_energy > _maxEnergy) _energy = _maxEnergy;
            if (_energy < 0) _energy = 0;
            
            transform.GetComponentInChildren<TextMeshProUGUI>().text = _energy.ToString();
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

    //���ݴ�� ������Ʈ
    private GameObject _attackObj;

    private Player _attackPlayer;

    //�ൿ��
    private int _stamina = 2;

    public enum AI_STATE
    {
        WAIT_L,
        WAIT_S,
        WAIT_R
    }
    //AI ���� �ʱ⼳��
    private AI_STATE _state = AI_STATE.WAIT_L;

    public void DataSet(int _id, ObjData aData)
    {
        id = _id;
        aiName = aData.DName;
        Position = aData.DPos;
        info = aData.DInfo;
        _maxEnergy = aData.DEnergy;
        Energy = aData.DEnergy;
    }

    /// <summary>
    /// AI �� ���� �Լ�
    /// </summary>
    public void AIMoveStart()
    {
        TargetOfAttackCheck();
        StartCoroutine(AIMove());
    }

    /// <summary>
    /// AI �����̴� �Լ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator AIMove()
    {
        _attackPlayer = _attackObj.GetComponent<Player>();

        if (_attackObj != gameObject) //���ݴ���� ���� ������Ʈ�� �ƴ϶�� (���ݴ���� ������ٸ�)
        {
            _stamina = 2; 
            while (_stamina > 0)
            {
                if (AttackDistanceCheck())
                {
                    _attackPlayer.Energy -= Energy;
                    break;
                }

                if (_pos.x == _attackPlayer.Position.x)
                {
                    Debug.Log("Y�̵�");
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
    /// Enemy ������Ʈ �̵� �Լ�
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
    /// ���ݴ�� ���� �Լ�
    /// </summary>
    /// <returns>���ݴ�� ���� ����</returns>
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
