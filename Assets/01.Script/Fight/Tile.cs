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
        else if (FightManager.Instance.MoveCheck(tile))
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
        FightManager.Instance.ClickTile(gameObject);     
    }
}
