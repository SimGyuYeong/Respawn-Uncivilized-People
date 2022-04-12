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
    /// 마우스 커서가 나갔을 때
    /// </summary>
    void OnMouseExit()
    {
        if (moveCheck())
            _highlight.SetActive(false);
    }

    /// <summary>
    /// 타일을 눌렀을 때 실행
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
                    FightManager.Instance.PlayerMove(tile.Position, transform);
                }
            }
        }
            
    }

    /// <summary>
    /// 플레이어가 움직일 수 있는 상태인지 체크
    /// </summary>
    /// <returns></returns>
    private bool moveCheck()
    {
        FightManager _fight = FightManager.Instance;

        if (_fight.turnType == FightManager.TurnType.Player
            && _fight.isClickPlayer
            && !_fight.move
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
