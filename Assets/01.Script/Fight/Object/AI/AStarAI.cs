using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AStarAI : AI
{
    protected override IEnumerator AIMoveState()
    {
        _attackPlayer = _attackObj.GetComponent<Player>();
        int _stamina = 1;

        if (_attackObj != gameObject) //공격대상이 본인 오브젝트가 아니라면 (공격대상이 지정됬다면)
        {
            FightManager.Instance.AStar.playerPos = new Vector2Int((int)_pos.x, (int)_pos.y);
            FightManager.Instance.AStar.targetPos = _attackPlayer.IPos;
            FightManager.Instance.AStar.PathFinding();

            while (true)
            {
                if (AttackDistanceCheck())
                {
                    _attackPlayer.DurabilityPoint -= InfluencePoint;
                    break;
                }

                if (_stamina == 4) break;

                Vector2 pos = new Vector2(FightManager.Instance.AStar.FinalNodeList[_stamina].x, FightManager.Instance.AStar.FinalNodeList[_stamina].y);
                transform.DOMove(pos, 1f);
                _pos = pos;

                _stamina++;
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
}
