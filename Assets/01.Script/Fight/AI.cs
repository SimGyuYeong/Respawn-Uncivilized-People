using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum AI_STATE
{
    WAIT_L = 1,
    WAIT_R,
    WAIT_S
}

public class AI : MonoBehaviour
{
    public AIInform ai;

    private GameObject _attackObj;
    private int _stamina = 2;
    private AI_STATE _state = AI_STATE.WAIT_L;

    public void AIMoveStart()
    {
        TargetOfAttackCheck();
        StartCoroutine(AIMove());
    }

    private IEnumerator AIMove()
    {
        Debug.Log(_state);   
        if (_attackObj != gameObject)
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
                    transform.position = ai.Position;
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
                    ++ai.Position.x;
                    transform.position = ai.Position;
                    if (FightManager.Instance.ObjCheck(ai.Position, 'r'))
                    {
                        _state = AI_STATE.WAIT_L;
                    }
                    break;
                case AI_STATE.WAIT_L:
                    --ai.Position.x;
                    transform.position = ai.Position;
                    if (FightManager.Instance.ObjCheck(ai.Position, 'l'))
                    {
                        _state = AI_STATE.WAIT_R;
                    }
                        
                    break;
                case AI_STATE.WAIT_S:
                    if (!FightManager.Instance.ObjCheck(ai.Position, 'l'))
                    {
                        --ai.Position.x;
                        transform.position = ai.Position;
                        _state = AI_STATE.WAIT_L;
                    }
                    else if (!FightManager.Instance.ObjCheck(ai.Position, 'r'))
                    {
                        ++ai.Position.x;
                        transform.position = ai.Position;
                        _state = AI_STATE.WAIT_R;
                    }

                    break;
            }
            yield return new WaitForSeconds(0.1f);
        }

        FightManager.Instance.move = false;
        FightManager.Instance.UpdateUI();
    }

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
