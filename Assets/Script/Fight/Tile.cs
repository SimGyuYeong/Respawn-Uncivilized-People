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
    /// ���콺 Ŀ���� ���� ������
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
    /// ���콺 Ŀ���� ������ ��
    /// </summary>
    void OnMouseExit()
    {
        if (moveCheck())
        {
            _highlight.SetActive(false);
        }
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
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// �÷��̾�� tPos���� �Ÿ�üũ
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
