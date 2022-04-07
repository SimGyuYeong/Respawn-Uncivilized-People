using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

enum AI_STATE
{
    WAIT_L = 1,
    WAIT_R,
    WAIT_S
}

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
                    _stamina--;
                    FightManager.Instance.energy -= ai.Health;
                    ai.Health /= 2;
                }
                else
                {
                    if (ai.Position.x == FightManager.Instance.playerPos.x)
                    {
                        if (ai.Position.y - FightManager.Instance.playerPos.y > 0)
                            ai.Position.y++;
                        else
                            ai.Position.y--;
                    }
                    else
                    {
                        if (ai.Position.x - FightManager.Instance.playerPos.x > 0)
                            ai.Position.x++;
                        else
                            ai.Position.x--;
                    }
                    _stamina--;
                    transform.position = new Vector3(ai.Position.x, ai.Position.y);
                }

                yield return new WaitForSeconds(0.8f);
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
    }

    /// <summary>
    /// Enemy ������Ʈ �̵� �Լ�
    /// </summary>
    /// <param name="value"></param>
    private void ObjMove(int value)
    {
        ai.Position.x += value;
        
        FightManager.Instance.enemyPos[0] = ai.Position;
        FightManager.Instance.tileList[ai.TileNum].tile.isEnemy = false;

        ai.TileNum += value;
        FightManager.Instance.tileList[ai.TileNum].tile.isEnemy = true;
        transform.SetParent(FightManager.Instance.tileList[ai.TileNum].gameObject.transform);

        transform.DOMoveX(ai.Position.x, 1F).OnComplete(() => FightManager.Instance.UpdateUI() );
    }

    /// <summary>
    /// ���ݴ�� ���� �Լ�
    /// </summary>
    /// <returns>���ݴ�� ���� ����</returns>
    private bool AttackDistanceCheck()
    {
        Vector2 _pos = ai.Position;

        _pos.x--;
        if (_pos == FightManager.Instance.playerPos)
            return true;

        _pos.x += 2;
        if (_pos == FightManager.Instance.playerPos)
            return true;

        _pos.x--;
        _pos.y++;
        if (_pos == FightManager.Instance.playerPos)
            return true;

        _pos.y -= 2;
        if (_pos == FightManager.Instance.playerPos)
            return true;

        return false;
    }

    private void TargetOfAttackCheck()
    {
        _attackObj = gameObject;

        Vector2 vec = ai.Position - FightManager.Instance.playerPos;
        float distance = Mathf.Abs(vec.x) + Mathf.Abs(vec.y);

        if (distance <= 3)
        {
            _attackObj = FightManager.Instance.Player;
        }
    }
}
