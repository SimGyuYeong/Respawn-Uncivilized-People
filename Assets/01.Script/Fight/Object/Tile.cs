using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject _highlight;
    public TileInform tile;

    private bool _isShowUI = false;

    /// <summary>
    /// 마우스 커서가 위에 있을때
    /// </summary>
    void OnMouseEnter()
    {
        if (FightManager.Instance.isIng == false && FightManager.Instance.pInput != FightManager.InputType.Input_Skill)
        {
            if (transform.childCount > 1)
            {
                _isShowUI = true;
                if (isPlayer())
                    FightManager.Instance.ShowUpdateStat(transform.GetChild(1).GetComponent<Player>());
                else if (isAI())
                    FightManager.Instance.ShowUpdateStat(transform.GetChild(1).GetComponent<AI>());
            }

            if (FightManager.Instance.turnType == FightManager.TurnType.Player)
            {
                _highlight.SetActive(true);
            }

            //공격
            if (FightManager.Instance.pInput == FightManager.InputType.Input_Skill)
            {
                if (isAI())
                {
                    if (Vector2Int.Distance(tile.Position, FightManager.Instance.pPos) <= 1)
                    {
                        FightManager.Instance.tPos = tile.Position;
                        FightManager.Instance.EnemyDraw();
                    }
                }
                else
                {
                    if (FightManager.Instance.lineRenderer.positionCount > 0)
                        FightManager.Instance.lineRenderer.positionCount = 0;
                }
            }
            else if (FightManager.Instance.MoveCheck(this))
            {
                FightManager.Instance.DrawLine();
            }
            else
                FightManager.Instance.lineRenderer.positionCount = 0;
        }
    }

    /// <summary>
    /// 마우스 커서가 나갔을 때
    /// </summary>
    void OnMouseExit()
    {
        _highlight.SetActive(false);

        if(_isShowUI)
        {
            _isShowUI = false;
            if (isPlayer())
                FightManager.Instance.UI.HidePlayerStatusUI();
            else
                FightManager.Instance.UI.HideAIStatusUI();
        }
    }

    /// <summary>
    /// 타일을 눌렀을 때 실행
    /// </summary>
    private void OnMouseDown()
    {
        FightManager.Instance.ClickTile(gameObject);
    }

    public bool isAI()
    {
        if(transform.childCount > 1)
        {
            return transform.GetChild(1).CompareTag("AI");
        }
        return false;
    }

    public bool isPlayer()
    {
        if (transform.childCount > 1)
        {
            return transform.GetChild(1).CompareTag("Player");
        }
        return false;
    }
}
