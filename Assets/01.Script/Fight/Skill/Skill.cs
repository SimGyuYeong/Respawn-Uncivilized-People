using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class Skill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] protected string _name;
    [SerializeField] protected int _cost;
    [SerializeField] protected int _damage;
    [SerializeField] protected int _range;
    [SerializeField] protected string _info;

    protected List<GameObject> _distanceTiles = new List<GameObject>();
    protected List<GameObject> _attackAI = new List<GameObject>();

    protected List<AI> _damagedAIList = new List<AI>();

    private void Awake()
    {
        transform.Find("Name").GetComponent<TextMeshProUGUI>().text = _name;
        transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = _cost.ToString();
    }

    /// <summary>
    /// ��ų ����� �����Ѱ�?
    /// </summary>
    /// <returns></returns>
    public bool SkillWhether()
    {
        if(FightManager.Instance.player.KineticPoint >= _cost)
        {
            if(CountingAI() > 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// ���ݰ����� AI ī����
    /// </summary>
    /// <returns></returns>
    protected virtual int CountingAI()
    {
        CheckDistance();
        foreach (var ai in _attackAI)
        {
            if (ai.GetComponentInChildren<AI>().isRestructuring == true)
            {
                _attackAI.Remove(ai);
            }
        }
        return _attackAI.Count;
    }

    /// <summary>
    /// ��Ÿ� ǥ��
    /// </summary>
    protected virtual void ShowDistance()
    {
        CheckDistance();
        _distanceTiles.ForEach(x => x.GetComponent<SpriteRenderer>().color = Color.red);
    }

    /// <summary>
    /// ��ų ��ư�� ������ ��
    /// </summary>
    protected virtual void SelectSkillButton()
    {
        if(SkillWhether() == true)
        {
            FightManager.Instance.player.KineticPoint -= _cost;
            FightManager.Instance.isSkillSelect = true;
            FightManager.Instance.UI.selectSkill = this;
            ShowDistance();
        }
    }

    /// <summary>
    /// ������ AI �������� ��
    /// </summary>
    /// <param name="ai"></param>
    public void SelectAI(AI ai)
    {
        _damagedAIList.Clear();
        _damagedAIList.Add(ai);
        AIDamage();
    }

    /// <summary>
    /// AI���� ������ ���ϱ�
    /// </summary>
    protected virtual void AIDamage()
    {
        FightManager.Instance.isSkillSelect = false;
        FightManager.Instance.HideDistance();
        _damagedAIList.ForEach(x => x.InfluencePoint -= _damage);
        FightManager.Instance.UI.ShowSkillUI(false);
        FightManager.Instance.NextPlayerTurn(FightManager.Action.Attack);
    }

    /// <summary>
    /// ��Ÿ��ȿ� �ִ� ��� Ÿ���� ����Ʈ�� �־���
    /// </summary>
    protected void CheckDistance()
    {
        _distanceTiles.Clear();
        _attackAI.Clear();

        foreach (var _tile in FightManager.Instance.tileList)
        {
            TileInform _tileInform = _tile.GetComponent<Tile>().tile;
            Transform _childTrm = transform;
            if (_tile.transform.childCount > 1)
            {
                _childTrm = _tile.transform.GetChild(1);
            }

            if (_tileInform.isWall == false)
            {
                FightManager.Instance.AStar.targetPos = _tileInform.Position;
                FightManager.Instance.AStar.PathFinding(true);

                if (FightManager.Instance.finalNodeList.Count <= _range + 1 && FightManager.Instance.finalNodeList.Count > 0)
                {
                    if(_childTrm.GetComponent<Player>() == null)
                    {
                        _distanceTiles.Add(_tile.gameObject);
                        if (_childTrm.GetComponent<AI>() != null)
                        {
                            _attackAI.Add(_tile.gameObject);
                        }
                    }
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(SkillWhether() == true)
        {
            SelectSkillButton();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(FightManager.Instance.isSkillSelect == false) ShowDistance();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (FightManager.Instance.isSkillSelect == false) FightManager.Instance.HideDistance();
    }
}
