//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Tile : MonoBehaviour
//{
//    public GameObject _highlight;
//    bool _active = true;
//    public int tileNum;

//    public void notActive()
//    {
//        _active = false;
//    }

//    public void enter(bool ck)
//    {
//        SpriteRenderer color = gameObject.GetComponent<SpriteRenderer>();
//        if (_active)
//        {
//            if (ck)
//            {
//                if (gameObject.transform.childCount >= 2)
//                    color.color = Color.red;
//                else
//                   color.color = Color.yellow;
//            }
                
//            else
//                color.color = Color.white;
//        }
//    }

//    void OnMouseEnter()
//    {
//        if (_active)
//            _highlight.SetActive(true);
//    }

//    void OnMouseExit()
//    {
//        if (_active)
//            _highlight.SetActive(false);
//    }

//    void OnMouseDown()
//    {
//        if(gameObject.transform.childCount >= 2)
//        {
//            if (gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().color == FightManager.Instance.Player.GetComponent<SpriteRenderer>().color)
//            {
//                FightManager.Instance.csTileNum = tileNum;
//                FightManager.Instance.OnClickPlayer();
//            }
//        }
        
//        if (FightManager.Instance._playerClick)
//        {
//            if (_active)
//            {
//                if(FightManager.Instance.csTileNum != tileNum)
//                    FightManager.Instance.PlayerMove(tileNum);
//            }
//        }
//    }
//}
