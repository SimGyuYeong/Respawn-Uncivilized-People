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
        if (moveCheck())
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
        if (moveCheck())
            _highlight.SetActive(false);
    }

    /// <summary>
    /// Ÿ���� ������ �� ����
    /// </summary>
    private void OnMouseDown()
    {
        if (!FightManager.Instance.move)
        {
            _highlight.SetActive(false);
            if (transform.childCount >= 2)
            {
                if (tile.Position == FightManager.Instance.playerPos)
                {
                    FightManager.Instance.ClickPlayer();
                    return;
                }
            }

            if(moveCheck())
            {
                if (FightManager.Instance.DistanceCheck(tile.Position))
                {
                    FightManager.Instance.ClickPlayer();
                    FightManager.Instance.targetPos = tile.Position;
                    FightManager.Instance.PathFinding();
                    FightManager.Instance.Player.transform.SetParent(transform);
                    StartCoroutine(FightManager.Instance.movePlayer());
                    FightManager.Instance.move = true;
                }
            }
        }
            
    }

    /// <summary>
    /// �÷��̾ ������ �� �ִ� �������� üũ
    /// </summary>
    /// <returns></returns>
    private bool moveCheck()
    {
        if (FightManager.Instance.isClickPlayer)
        {
            if (!FightManager.Instance.move)
            {
                if (!tile.isWall)
                {
                    if(FightManager.Instance.DistanceCheck(tile.Position))
                        return true;
                }
            }
        }
        return false;
    }

    //void OnMouseDown()
    //{
    //    if (gameObject.transform.childCount >= 2)
    //    {
    //        if (gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().color == FightManager.Instance.Player.GetComponent<SpriteRenderer>().color)
    //        {
    //            FightManager.Instance.csTileNum = tileNum;
    //            FightManager.Instance.OnClickPlayer();
    //        }
    //    }

    //    if (FightManager.Instance._playerClick)
    //    {
    //        if (_active)
    //        {
    //            if (FightManager.Instance.csTileNum != tileNum)
    //                FightManager.Instance.PlayerMove(tileNum);
    //        }
    //    }
    //}
}
