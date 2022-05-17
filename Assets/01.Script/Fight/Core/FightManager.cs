using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class FightManager : MonoBehaviour
{
    //�̱���
    public static FightManager Instance;

    #region A* �˰���
    
    private AStarAlgorithm _aStar; // A* �˰��� ĳ��

    public AStarAlgorithm AStar => _aStar;

    //A* �˰��򿡼� �������� ������
    private List<Node> _finalNodeList { get => _aStar.FinalNodeList; }

    //A* �˰��� ��ϵ� �÷��̾� �δ��� ��ǥ�� �������ų� ����
    public Vector2Int pPos
    {
        get => _aStar.playerPos;
        set => _aStar.playerPos = value;
    }

    //A* �˰��� ��ϵ� Ÿ�� ��ǥ�� �������ų� ����
    public Vector2Int tPos
    {
        get => _aStar.targetPos;
        set => _aStar.targetPos = value;
    }
    #endregion

    public List<ObjData> playerDataList = new List<ObjData>();

    public List<Vector2Int> enemyPos = new List<Vector2Int>(); //������ ��ǥ ����Ʈ
    private List<int> _noneEnemyList = new List<int>(); //�������� ���� ���� ����Ʈ


    //Ÿ���� ������ ������Ʈ �θ�, �÷��̾� ������, �÷��̾ �����̴� �ִϸ��̼� ������Ʈ
    public GameObject content, moveAni;

    public Tile tilePrefab;
    public AI enemyPrefab;
    public Player playerPrefab;

    public AI ai;
    public Player player;

    //���� �÷��̾� �ִϸ��̼��� �۵�������
    public bool isIng = false;

    //�� ���� ����Ʈ
    public bool[] isWallList;
    
    //�÷��̾ ���Ͽ� ��� �ൿ�� �� �ִ���
    private int _turnPlayerCount = 0;

    //Ÿ�� ����Ʈ
    public List<Tile> tileList = new List<Tile>();

    //�� ����Ʈ
    public List<AI> enemyList = new List<AI>();

    //�÷��̾� ����Ʈ
    private List<Player> _playerList = new List<Player>();

    //�׼� ��ư ������Ʈ
    [SerializeField] private GameObject _actionButton;

    public int turn = 10;

    public bool isClickPlayer = false;
    public int moveDistance = 4;

    public LineRenderer lineRenderer;

    [field: SerializeField] public UnityEvent OnUIChange { get; set; }

    public enum TurnType
    {
        Wait_AI = 0,
        AI,
        Wait_Player,
        Input_Action,
        Player_Attack,
        Player_Move
    }

    public TurnType turnType = TurnType.Wait_Player;

    public enum Action
    {
        Move = 0,
        Attack = 1,
        Stop = 2
    }

    private void Awake()
    {
        Instance = this;
        lineRenderer = GetComponent<LineRenderer>();
        _aStar = GetComponent<AStarAlgorithm>();
            
    }

    private void Start()
    {
        StartCoroutine(spawnTile());
        lineRenderer.startWidth = .05f;
        lineRenderer.endWidth = .05f;
    }

    private void PlayerSpawn()
    {
        int count = 1;
        foreach (var p in playerDataList)
        {
            int slot = Mathf.FloorToInt((7 - p.DPos.y) * 8 + p.DPos.x);

            var _player = Instantiate(playerPrefab, tileList[slot].transform);
            _player.DataSet(count, p);
            _turnPlayerCount += 2;

            _playerList.Add(_player);
            count++;
        }
    }

    /// <summary>
    /// Ÿ�� ���� �ڷ�ƾ �Լ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator spawnTile()
    {
        int count = 1, aiCount = 0;
        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                spawnedTile.transform.parent = content.transform;
                spawnedTile.name = $"Tile {count}";
                spawnedTile.tile = new TileInform(count, x, y);

                tileList.Add(spawnedTile);

                foreach(Vector2Int pos in enemyPos)
                {
                    if(pos == new Vector2Int(x, y))
                    {
                        AI _ai = Instantiate(enemyPrefab, spawnedTile.transform);
                        _ai.transform.position = spawnedTile.transform.position;
                        spawnedTile.tile.isEnemy = true;
                        int _c = count;
                        _ai.ai = new AIInform(aiCount, x, y, 45, count-1);
                        enemyList.Add(_ai);
                        aiCount++;
                    }
                }

                if (isWallList[count - 1] == true)
                {
                    spawnedTile.tile.isWall = true;
                    spawnedTile.GetComponent<SpriteRenderer>().color = Color.gray;
                }

                count++;
                if (y == 7) yield return new WaitForSeconds(0.08f);
            }
            yield return new WaitForSeconds(0.08f);
        }
        PlayerSpawn(); 

        OnUIChange?.Invoke();
        TurnChange();
    }

    #region ���� �׸���
    public void DrawLine()
    {
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.positionCount = 0;
        for (int i = 0; i < _finalNodeList.Count; i++)
        {
            Vector2 pos = new Vector2(_finalNodeList[i].x, _finalNodeList[i].y);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(i, pos);
        }
    }

    public void EnemyDraw()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        lineRenderer.SetPosition(0, new Vector3(pPos.x, pPos.y));
        lineRenderer.SetPosition(1, new Vector3(tPos.x, tPos.y));
    }
    #endregion

    #region Check
    /// <summary>
    /// �̵������� �Ÿ����� üũ�ϴ� �Լ�
    /// </summary>
    /// <param name="pos">�̵��� ��ǥ</param>
    /// <returns>�̵������ϸ� True, �Ұ����ϸ� False</returns>
    public bool DistanceCheck(Vector2 pos)
    {
        tPos = new Vector2Int((int)pos.x, (int)pos.y);
        _aStar.PathFinding();
        if (_finalNodeList.Count <= moveDistance+1 && _finalNodeList.Count > 0) return true;
        return false;
    
        
    }
     
    /// <summary>
    /// �������� üũ �Լ�
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>�������� ���� ��ȯ</returns>
    public bool ObjCheck(Vector2 pos, char value)
    {
        switch(value)
        {
            case 'r':
                pos.x++;
                break;
            case 'l':
                pos.x--;
                break;
            case 'u':
                pos.y++;
                break;
            case 'd':
                pos.y--;
                break;
        }    

        if (pos.x < 0 || pos.x > 7 || pos.y > 7 || pos.y < 0)
            return true;

        if (_aStar.NodeArray[(int)pos.x, (int)pos.y].isWall)
            return true;

        foreach(var p in enemyPos)
        {
            if (p == pos)
                return true;
        }

        return false;
    }

    /// <summary>
    /// ������ �������� üũ�ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    private bool AttackCheck(Vector2 matchPos)
    {
        if (player.isFight == false)
        {
            foreach (Vector2 pos in enemyPos)
            {
                if (Vector2.Distance(pos, matchPos) <= 1) return true;
            }
        }

        return false;
    }
    #endregion

    public void ShowDistance(string type)
    {
        SpriteRenderer _spriteRenderer;

        foreach(var _tile in tileList)
        {
            TileInform _tileInform = _tile.GetComponent<Tile>().tile;

            if(_tileInform.isWall == false)
            {
                _spriteRenderer = _tile.GetComponent<SpriteRenderer>();

                if (type == "Move")
                {
                    if (DistanceCheck(_tileInform.Position))
                    {
                        _spriteRenderer.color = Color.yellow;
                    }
                }
                else if (type == "Attack")
                {
                    if (Vector2.Distance(_tileInform.Position, pPos) <= 1)
                    {
                        _spriteRenderer.color = Color.red;
                    }
                }
            }
        }
    }
    public void HideDistance()
    {
        SpriteRenderer _spriteRenderer;
        int count = 0;

        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                if (!isWallList[count])
                {
                    _spriteRenderer = tileList[count].GetComponent<SpriteRenderer>();
                    _spriteRenderer.color = Color.white;
                }
                count++;
            }
        }
    }

    /// <summary>
    /// ĳ���� �̵� �Լ�
    /// </summary>
    public void PlayerMove(Vector2Int pos, Transform parent)
    {
        ClickPlayer();
        tPos = pos;
        _aStar.PathFinding();

        player.isMove = true;
        player.transform.SetParent(parent);
        
        StartCoroutine(PlayerMoveCoroutine());
    }

    /// <summary>
    /// �÷��̾� �̵� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayerMoveCoroutine()
    {
        isIng = true;
        lineRenderer.positionCount = 0;
        player.gameObject.SetActive(false);

        for(int i = 0; i < _finalNodeList.Count; i++)
        {
            var obj = Instantiate(moveAni);
            obj.transform.position = new Vector2(_finalNodeList[i].x, _finalNodeList[i].y);
            player.Energy -= 2;
            yield return new WaitForSeconds(0.2f);
        }
        player.Energy += 2;
        player.Position = tPos;
        player.gameObject.SetActive(true);
        OnUIChange?.Invoke();

        yield return new WaitForSeconds(2f);
        NextPlayerTurn(Action.Move);
    }

    /// <summary>
    /// �÷��̾� �ൿ �� �����Լ�
    /// </summary>
    private void NextPlayerTurn(Action action)
    {
        _turnPlayerCount--;
        isIng = false;
        if (_turnPlayerCount > 0)
        {
            if(action == Action.Move) player.isMove = true;
            else if (action == Action.Attack) player.isFight = true; 

            bool isNext = true;
            foreach (var p in _playerList)
            {
                if (!p.isMove)
                {
                    isNext = false;
                }
                else if (!p.isFight)
                {
                    if (AttackCheck(p.Position))
                    {
                        isNext = false;
                    }
                }
            }

            if(isNext == false)
            {
                turnType = TurnType.Input_Action;
                return;
            }
                
        }

        TurnChange();
    }

    /// <summary>
    /// �÷��̾ Ŭ������ ��
    /// </summary>
    public void ClickPlayer(Player _clickPlayer = null)
    {
        if (_clickPlayer == null) _clickPlayer = player;

        if(isClickPlayer)
        {
            if (player == _clickPlayer)
            {
                isClickPlayer = false;
                lineRenderer.positionCount = 0;
                _actionButton.SetActive(false);
                HideDistance();
                return;
            }
            else
            {
                _actionButton.SetActive(false);
            }
        }

        isClickPlayer = true;

        player = _clickPlayer;
        pPos = player.IPos;

        _actionButton.transform.position = new Vector3(pPos.x, pPos.y);
        _actionButton.SetActive(true);

        Transform moveTrm = _actionButton.transform.Find("Move");
        Transform attackTrm = _actionButton.transform.Find("Attack");


        TextMeshProUGUI moveText = moveTrm.GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI fightText = attackTrm.GetComponentInChildren<TextMeshProUGUI>();

        Image image = moveTrm.GetComponent<Image>();
        Color c = image.color;
        if (player.isMove)
        {
            moveText.color = Color.red;
            c.a = 0.5f;
        }
        else
        {
            moveText.color = Color.black;
            c.a = 1f;
        }
        image.color = c;

        image = attackTrm.GetComponent<Image>();
        c = image.color;
        if (player.isFight || AttackCheck(pPos) == false)
        {
            fightText.color = Color.red;
            c.a = 0.5f;
        }

        else
        {
            fightText.color = Color.black;
            c.a = 1f;
        }
        image.color = c;
    }

    public void PlayerAction(Action value)
    {
        if (value == Action.Attack && AttackCheck(pPos) == false)
            return;

        _actionButton.SetActive(false);
        switch(value)
        {
            case Action.Move:
                ShowDistance("Move");
                turnType = TurnType.Player_Move;
                break;
            case Action.Attack:
                ShowDistance("Attack");
                turnType = TurnType.Player_Attack;
                break;
            case Action.Stop:
                turnType = TurnType.Player_Move;
                TurnChange();
                break;
            default:
                break;
        }
    }

    public void PlayerAttack(int enemyCount)
    {
        player.Energy -= enemyList[enemyCount].ai.Health;
        enemyPos.RemoveAt(enemyCount);
        
        if(_noneEnemyList.Count > 0)
        {
            _noneEnemyList.RemoveAt(enemyCount);
        }
        
        Destroy(enemyList[enemyCount].gameObject);
        enemyList.RemoveAt(enemyCount);

        int num = 0;
        foreach(var e in enemyList)
        {
            AI _ai = e.GetComponent<AI>();
            if (_ai.ai.Number - num != 0)
            {
                _ai.ai.Number--;
            }
            num++;
        }
        ClickPlayer();
        OnUIChange?.Invoke();
        StartCoroutine(AfterAttack());
    }

    IEnumerator AfterAttack()
    {
        yield return new WaitForSeconds(2f);
        NextPlayerTurn(Action.Attack);
    }

    public void TurnChange()
    {
        if(turn==0)
        {
            Debug.Log("��������");
            return;
        }

        switch (turnType)
        {
            case TurnType.Wait_Player:
                Debug.Log("Player Turn");
                turnType = TurnType.Input_Action;
                break;

            case TurnType.Player_Move:
            case TurnType.Player_Attack:
                turnType = TurnType.Wait_AI;
                TurnChange();
                break;

            case TurnType.Wait_AI:
                Debug.Log("AI Turn");
                turnType = TurnType.AI;

                //�������� �ʴ� ���� ���ٸ�
                if(_noneEnemyList.Count <= 0)
                {
                    //��� ������ �������� �ʴ� �� ����Ʈ�� �ٽ� �ִ´�.
                    for (int i = 0; i < enemyPos.Count; i++)
                        _noneEnemyList.Add(i);
                }

                int _num = Random.Range(0, _noneEnemyList.Count);
                enemyList[_noneEnemyList[_num]].AIMoveStart(); //�������� ������ ���� �����δ�.
                _noneEnemyList.RemoveAt(_num); //������ ���� �������� ���� �� ����Ʈ���� ����
                if (_noneEnemyList.Count < 1) //��� ���� �������ٸ�
                    _noneEnemyList.Clear(); //�������� ���� �� ����Ʈ �ʱ�ȭ

                break;

            case TurnType.AI:
                turn--;
                StartCoroutine(playerTurn());
                break;
        }
    }

    private IEnumerator playerTurn()
    {
        yield return null;   
        turnType = TurnType.Wait_Player;

        foreach(var p in _playerList)
        {
            p.isFight = false;
            p.isMove = false;
        }

        TurnChange();
    }

    public void ClickTile(GameObject tileObj)
    {
        GameObject highLight = tileObj.transform.Find("_highlight").gameObject;
        TileInform tile = tileObj.GetComponent<Tile>().tile;

        if (isIng == false)
        {
            highLight.SetActive(false);
            if (turnType == TurnType.Input_Action)
            {
                if(tileObj.transform.childCount >= 2)
                {
                    if(tileObj.transform.GetComponentInChildren<Player>() != null)
                    {
                        ClickPlayer(tileObj.transform.GetChild(1).GetComponent<Player>());
                    }
                }
            }

            else if (turnType == TurnType.Player_Move)
            {
                if (tile.Position == pPos)
                {
                    turnType = TurnType.Input_Action;
                    HideDistance();
                    return;
                }

                if (MoveCheck(tile) && DistanceCheck(tile.Position))
                {
                    PlayerMove(tile.Position, tileObj.transform);
                }
            }
            else if (turnType == TurnType.Player_Attack)
            {
                if (tile.Position == pPos)
                {
                    turnType = TurnType.Input_Action;
                    HideDistance();
                    return;
                }

                if (tile.isEnemy)
                {
                    if (Vector2Int.Distance(tile.Position, pPos) <= 1)
                        PlayerAttack(tileObj.GetComponentInChildren<AI>().ai.Number);
                }
            }
        }
    }

    /// <summary>
    /// �÷��̾ ������ �� �ִ� �������� üũ
    /// </summary>
    /// <returns>������ �� �ִٸ� True, �ƴϸ� False</returns>
    public bool MoveCheck(TileInform tile)
    {
        if (turnType == TurnType.Player_Move
            && isClickPlayer
            && isIng == false
            && tile.isWall == false)
        {
            if (tile.isEnemy)
            {
                if (DistanceCheck(tile.Position))
                    return true;
            }
            else
            {
                if (DistanceCheck(tile.Position))
                    return true;
            }
        }
        return false;
    }

    public void ShowUpdateStat(Player _player)
    {
        UIManager.Instance.ShowStatUI(_player.name, _player.Energy, _player.info);
    }

    public void ShowUpdateStat(AI _ai)
    {
        //UIManager.Instance.ShowStatUI(_ai);
    }
}
