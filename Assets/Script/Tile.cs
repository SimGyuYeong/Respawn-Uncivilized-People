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

    public void enter(bool ck)
    {
        if(FightManager.Instance.move == false)
        {
            SpriteRenderer color = gameObject.GetComponent<SpriteRenderer>();
            if (!tile.isWall)
            {
                if (ck)
                {
                    if (gameObject.transform.childCount >= 2)
                        color.color = Color.red;
                    else
                        color.color = Color.yellow;
                }

                else
                    color.color = Color.white;
            }
        }
        
    }

    void OnMouseEnter()
    {
        if (FightManager.Instance.move == false)
        {
            if (!tile.isWall)
            {
                _highlight.SetActive(true);
                FightManager.Instance.targetPos.x = tile.x;
                FightManager.Instance.targetPos.y = tile.y;
                FightManager.Instance.PathFinding();
                FightManager.Instance.DrawLine();
            }
        }
    }

    void OnMouseExit()
    {
        if (FightManager.Instance.move == false)
        {
            if (!tile.isWall)
                _highlight.SetActive(false);
        }
            
    }

    private void OnMouseDown()
    {
        if (FightManager.Instance.move == false)
        {
            _highlight.SetActive(false);
            FightManager.Instance.move = true;
            StartCoroutine(FightManager.Instance.movePlayer());
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
