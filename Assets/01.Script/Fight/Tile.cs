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
    /// 마우스 커서가 나갔을 때
    /// </summary>
    void OnMouseExit()
    {
        if (MoveCheck())
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
                if (MoveCheck() && FightManager.Instance.DistanceCheck(tile.Position))
                {
                    FightManager.Instance.PlayerMove(tile.Position, transform);
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
