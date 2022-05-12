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
        else if (FightManager.Instance.MoveCheck(tile))
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
        FightManager.Instance.ClickTile(gameObject);     
    }
}
