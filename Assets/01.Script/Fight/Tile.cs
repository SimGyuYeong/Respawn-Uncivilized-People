using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject _highlight;
    public TileInform tile;

    /// <summary>
    /// 마우스 커서가 위에 있을때
    /// </summary>
    void OnMouseEnter()
    {
        if(FightManager.Instance.turnType == FightManager.TurnType.Player_Move || FightManager.Instance.turnType == FightManager.TurnType.Player_Attack)
        {
            _highlight.SetActive(true);
        }

        if(FightManager.Instance.turnType == FightManager.TurnType.Player_Attack)
        {
            if (tile.isEnemy)
            {
                if (Vector2Int.Distance(tile.Position, FightManager.Instance.pPos) <= 1)
                {
                    FightManager.Instance.tPos = tile.Position;
                    FightManager.Instance.EnemyDraw();
                }
            }
            else
            {
                if(FightManager.Instance.lineRenderer.positionCount > 0)
                    FightManager.Instance.lineRenderer.positionCount = 0;
            }
        }
        else if (MoveCheck())
        {
            FightManager.Instance.DrawLine();
        }
        else
            FightManager.Instance.lineRenderer.positionCount = 0;
    }

    /// <summary>
    /// 마우스 커서가 나갔을 때
    /// </summary>
    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    /// <summary>
    /// 타일을 눌렀을 때 실행
    /// </summary>
    private void OnMouseDown()
    {
        if (!FightManager.Instance.isIng)
        {
            _highlight.SetActive(false);
            if (FightManager.Instance.turnType == FightManager.TurnType.Input_Action)
            {
                
                if (tile.Position == FightManager.Instance.pPos)
                {
                    FightManager.Instance.ClickPlayer();
                    return;
                }
            }
            else if (FightManager.Instance.turnType == FightManager.TurnType.Player_Move)
            {
                if (tile.Position == FightManager.Instance.pPos)
                {
                    FightManager.Instance.turnType = FightManager.TurnType.Input_Action;
                    FightManager.Instance.HideDistance();
                    return;
                }

                if (MoveCheck() && FightManager.Instance.DistanceCheck(tile.Position))
                {
                    FightManager.Instance.PlayerMove(tile.Position, transform);
                }
            }
            else if (FightManager.Instance.turnType == FightManager.TurnType.Player_Attack)
            {
                if (tile.Position == FightManager.Instance.pPos)
                {
                    FightManager.Instance.turnType = FightManager.TurnType.Input_Action;
                    FightManager.Instance.HideDistance();
                    return;
                }

                if (tile.isEnemy)
                {
                    if (Vector2Int.Distance(tile.Position, FightManager.Instance.pPos) <= 1)
                        FightManager.Instance.PlayerAttack(transform.GetChild(1).GetComponent<AI>().ai.Number);
                }
            }
        }
            
    }

    /// <summary>
    /// 플레이어가 움직일 수 있는 상태인지 체크
    /// </summary>
    /// <returns>움직일 수 있다면 True, 아니면 False</returns>
    private bool MoveCheck()
    {
        FightManager _fight = FightManager.Instance;

        if (_fight.turnType == FightManager.TurnType.Player_Move
            && _fight.isClickPlayer
            && !_fight.isIng
            && !tile.isWall)
        {
            if (tile.isEnemy)
            {
                if (_fight.DistanceCheck(tile.Position))
                    return true;
            }
            else
            {
                if (_fight.DistanceCheck(tile.Position))
                    return true;
            }
        }
        return false;
    }
}
