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
        if (MoveCheck())
        {
            _highlight.SetActive(true);
            if (tile.isEnemy)
                FightManager.Instance.EnemyDraw();
            else
                FightManager.Instance.DrawLine();
        }
        else
            FightManager.Instance._lineRenderer.positionCount = 0;
    }

    /// <summary>
    /// ���콺 Ŀ���� ������ ��
    /// </summary>
    void OnMouseExit()
    {
        if (MoveCheck())
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
                if (MoveCheck() && FightManager.Instance.DistanceCheck(tile.Position))
                {
                    FightManager.Instance.PlayerMove(tile.Position, transform);
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
