using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileInform
{
    public TileInform(int _num, int _x, int _y, bool _wall)
    {
        tileNum = _num;
        x = _x;
        y = _y;
        isWall = _wall;
    }

    public int tileNum, x, y;
    public bool isWall;
}

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
            if(distanceCheck(transform.position))
            {
                _highlight.SetActive(true);
                FightManager.Instance.targetPos = new Vector2Int(tile.x, tile.y);
                FightManager.Instance.PathFinding();
                FightManager.Instance.DrawLine();
            }
        }
    }

    /// <summary>
    /// 마우스 커서가 나갔을 때
    /// </summary>
    void OnMouseExit()
    {
        if (moveCheck())
        {
            _highlight.SetActive(false);
        }
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
                if (transform.GetChild(1).GetComponent<SpriteRenderer>().color == FightManager.Instance.Player.GetComponent<SpriteRenderer>().color)
                {
                    FightManager.Instance.ClickPlayer();
                    return;
                }
            }

            if(moveCheck())
            {
                if (distanceCheck(transform.position))
                {
                    FightManager.Instance.ClickPlayer();
                    FightManager.Instance.Player.transform.SetParent(transform);
                    StartCoroutine(FightManager.Instance.movePlayer());
                    FightManager.Instance.move = true;
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
        if (FightManager.Instance.isClickPlayer)
        {
            if (!FightManager.Instance.move)
            {
                if (!tile.isWall)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 플레이어와 tPos와의 거리체크
    /// </summary>
    /// <param name="tPos"></param>
    /// <returns></returns>
    private bool distanceCheck(Vector2 tPos)
    {
        Vector2 pPos = FightManager.Instance.playerPos;
        Vector2 dPos = tPos - pPos;
        float distance = Mathf.Abs(dPos.x) + Mathf.Abs(dPos.y);
        if(distance <= FightManager.Instance.distance)
        {
            return true;
        }
        else
        {
            return false;
        }
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
