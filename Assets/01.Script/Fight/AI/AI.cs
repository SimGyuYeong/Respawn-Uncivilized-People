using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AI : MonoBehaviour
{
    public AIInform ai;

    //���ݴ�� ������Ʈ
    private GameObject _attackObj;

    //�ൿ��
    private int _stamina = 2;

    //AI ���� �ʱ⼳��
    private AI_STATE _state = AI_STATE.WAIT_L;

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

        if (_attackObj != gameObject) //���ݴ���� ���� ������Ʈ�� �ƴ϶�� (���ݴ���� ������ٸ�)
        {
            _stamina = 2; 
            while (_stamina > 0)
            {
                if (AttackDistanceCheck())
                {
                    FightManager.Instance.Energy -= ai.Health/2;
                    ai.Health /= 2;
                    break;
                }
                else
                {
                    if (ai.Position.x == FightManager.Instance.pPos.x)
                    {
                        if (ai.Position.y - FightManager.Instance.pPos.y > 0)
                        {
                            if (!FightManager.Instance.ObjCheck(ai.Position, 'd'))
                                ObjMove(8);
                        }
                            
                        else
                        {
                            if (!FightManager.Instance.ObjCheck(ai.Position, 'u'))
                                ObjMove(-8);
                        }
                            
                    }
                    else
                    {
                        if (ai.Position.x - FightManager.Instance.pPos.x > 0)
                        {
                            
                            if (!FightManager.Instance.ObjCheck(ai.Position, 'l'))
                                ObjMove(-1);
                        }
                            
                        else
                        {
                            if (!FightManager.Instance.ObjCheck(ai.Position, 'r'))
                                ObjMove(1);
                        }
                            
                    }
                    _stamina--;
                }

                yield return new WaitForSeconds(1f);
            }
        }

        else
        {

            if (FightManager.Instance.ObjCheck(ai.Position, 'r')
                && FightManager.Instance.ObjCheck(ai.Position, 'l'))
                {
                _state = AI_STATE.WAIT_S;
            }

            switch (_state)
            {
                case AI_STATE.WAIT_R:
                    ObjMove(1);
                    if (FightManager.Instance.ObjCheck(ai.Position, 'r'))
                        _state = AI_STATE.WAIT_L;

                    break;

                case AI_STATE.WAIT_L:
                    ObjMove(-1);
                    if (FightManager.Instance.ObjCheck(ai.Position, 'l'))
                        _state = AI_STATE.WAIT_R;
                        
                    break;

                case AI_STATE.WAIT_S:
                    if (!FightManager.Instance.ObjCheck(ai.Position, 'l'))
                    {
                        ObjMove(-1);
                        _state = AI_STATE.WAIT_L;
                    }

                    else if (!FightManager.Instance.ObjCheck(ai.Position, 'r'))
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
            ai.Position.x += value;
        else if (value == 8)
            ai.Position.y--;
        else
            ai.Position.y++;

        FightManager.Instance.enemyPos[ai.Number] = ai.Position;
        FightManager.Instance.tileList[ai.TileNum].tile.isEnemy = false;

        ai.TileNum += value;
        FightManager.Instance.tileList[ai.TileNum].tile.isEnemy = true;
        transform.SetParent(FightManager.Instance.tileList[ai.TileNum].gameObject.transform);

        transform.DOMove(new Vector3(ai.Position.x, ai.Position.y), 1F);
    }

    /// <summary>
    /// ���ݴ�� ���� �Լ�
    /// </summary>
    /// <returns>���ݴ�� ���� ����</returns>
    private bool AttackDistanceCheck()
    {
        Vector2 _pos = ai.Position;

        int distance = (int)Vector2.Distance(_pos, FightManager.Instance.pPos);
        if (distance == 1)
            return true;

        return false;
    }

    private void TargetOfAttackCheck()
    {
        _attackObj = gameObject;

        int distance = (int)Vector2.Distance(ai.Position, FightManager.Instance.pPos);
        if (distance <= 3)
        {
            _attackObj = FightManager.Instance.Player;
        }
    }
}
