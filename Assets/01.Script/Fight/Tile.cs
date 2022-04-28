using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject _highlight;
    public TileInform tile;

    /// <summary>
    /// ���콺 Ŀ���� ���� ������
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
    /// ���콺 Ŀ���� ������ ��
    /// </summary>
    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    /// <summary>
    /// Ÿ���� ������ �� ����
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
    /// �÷��̾ ������ �� �ִ� �������� üũ
    /// </summary>
    /// <returns>������ �� �ִٸ� True, �ƴϸ� False</returns>
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
