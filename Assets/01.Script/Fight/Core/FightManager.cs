using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class FightManager : MonoBehaviour
{
    //�̱���
    public static FightManager Instance;

    private Tutorial _tuto;
    private UIManager _uiManager;
    public UIManager UI { get => _uiManager; }

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

    public List<PlayerData> playerDataList = new List<PlayerData>();
    public List<AIData> aiDataList = new List<AIData>();


    //Ÿ���� ������ ������Ʈ �θ�, �÷��̾� ������, �÷��̾ �����̴� �ִϸ��̼� ������Ʈ
    public GameObject content, moveAni;

    public Tile tilePrefab;
    public AI enemyPrefab;
    public Player playerPrefab;

    public Player player;

    //���� �÷��̾� �ִϸ��̼��� �۵�������
    public bool isIng = false;

    //�� ���� ����Ʈ
    public bool[] isWallList;
    
    //�÷��̾ ���Ͽ� ��� �ൿ�� �� �ִ���
    private int _maxTurnCount = 0;
    private int _turnCount = 0;

    public List<Tile> tileList = new List<Tile>();
    public List<AI> aiList = new List<AI>();
    public List<Player> playerList = new List<Player>();

    //�׼� ��ư ������Ʈ
    [SerializeField] private GameObject _actionButton;

    public int maxTurn = 10;
    public int turn = 0;

    public bool isClickPlayer = false;
    public int moveDistance = 4;

    private int _pCount = 0;
    private int _aiMoveCount = 0;

    public static int sendChatID = 0;

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
        lineRenderer.startWidth = .05f;
        lineRenderer.endWidth = .05f;

        _aStar = GetComponent<AStarAlgorithm>();
        _tuto = transform.parent.Find("Tutorial").GetComponent<Tutorial>();
        _uiManager = transform.parent.Find("UIManager").GetComponent<UIManager>();
    }

    private void Start()
    {
        StartCoroutine(TileSpawn());
        isIng = true;
        turn = maxTurn;
    }

    private void PlayerSpawn()
    {
        int count = 0;
        foreach (var p in playerDataList)
        {
            int slot = Mathf.FloorToInt((7 - p.position.y) * 8 + p.position.x);

            var _player = Instantiate(playerPrefab, tileList[slot].transform);
            _player.Init(count, p);
            _maxTurnCount += 2;

            _player.transform.localScale = Vector2.one * 1.5f;

            Sequence seq = DOTween.Sequence();
            seq.Append(_player.transform.GetComponent<Image>().DOFade(1f, 0.2f));
            seq.Join(_player.transform.DOScale(Vector2.one * 0.8f, 0.2f));
            seq.AppendCallback(() => AISpawn());

            playerList.Add(_player);
            count++;
        }
        _turnCount = _maxTurnCount;
    }

    private void AISpawn()
    {
        _pCount -= 1;
        if (_pCount > 0) return;

        int count = 0;
        foreach(var ai in aiDataList)
        {
            int slot = Mathf.FloorToInt((7 - ai.position.y) * 8 + ai.position.x);

            var _ai = aiList[count];

            _ai.gameObject.SetActive(true);
            _ai.Init(count, ai);

            _ai.transform.localScale = Vector2.one * 1.5f;

            Sequence seq = DOTween.Sequence();
            seq.Append(_ai.transform.GetComponent<SpriteRenderer>().DOFade(1f, 0.2f));
            seq.Join(_ai.transform.DOScale(Vector2.one * 0.8f, 0.2f));
            
            count++;
        }
    }

    /// <summary>
    /// Ÿ�� ���� �ڷ�ƾ �Լ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator TileSpawn(bool tuto = true)
    {
        int count = 1;
        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                var spawnedTile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                spawnedTile.transform.parent = content.transform;
                spawnedTile.name = $"Tile {count}";
                spawnedTile.tile = new TileInform(count, x, y);

                tileList.Add(spawnedTile);

                if (isWallList[count - 1] == true)
                {
                    spawnedTile.tile.isWall = true;
                    spawnedTile.GetComponent<SpriteRenderer>().color = Color.gray;
                }

                count++;
                if (y == 7) yield return new WaitForSeconds(0.06f);
            }
            yield return new WaitForSeconds(0.06f);
        }

        _pCount = playerDataList.Count;
        PlayerSpawn();

        foreach(var ai in aiDataList)
        {
            int slot = Mathf.FloorToInt((7 - ai.position.y) * 8 + ai.position.x);

            var _ai = Instantiate(enemyPrefab, tileList[slot].transform);
            _ai.gameObject.SetActive(false);

            aiList.Add(_ai);
        }

        _aStar.PathFinding();
        OnUIChange?.Invoke();

        if (tuto)
            StartCoroutine(_tuto.StartTutorial());
        else
        {
            TurnChange();
        }
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

        int slot = Mathf.FloorToInt((7 - pos.y) * 8 + pos.x);
        if (tileList[slot].transform.childCount > 1)
        {
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
            foreach (var ai in aiList)
            {
                if (Vector2.Distance(ai.Position, matchPos) <= 1) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// �÷��̾ ������ �� �ִ� �������� üũ
    /// </summary>
    /// <returns>������ �� �ִٸ� True, �ƴϸ� False</returns>
    public bool MoveCheck(Tile tile)
    {
        if (turnType == TurnType.Player_Move
            && isClickPlayer
            && isIng == false
            && tile.tile.isWall == false)
        {
            if (tile.isAI() == false)
            {
                if (DistanceCheck(tile.tile.Position))
                    return true;
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
            player.DurabilityPoint -= 2;
            yield return new WaitForSeconds(0.2f);
        }
        player.DurabilityPoint += 2;
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
        _turnCount--;
        isIng = false;
        if (_turnCount > 0)
        {
            if (action == Action.Move)
            {
                player.isMove = true;
                if (player.isFight == false && AttackCheck(player.IPos))
                {
                    return;
                }
            }
            else if (action == Action.Attack)
            {
                player.isFight = true;
                if(player.isMove == false)
                {
                    return;
                }
            }
        }

        turnType = TurnType.Input_Action;
        _uiManager.TurnstopEmphasis();
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

        if (value == Action.Move && player.isMove == true)
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

    public void PlayerAttack(int aiID)
    {
        player.DurabilityPoint -= aiList[aiID].InfluencePoint;

        Destroy(aiList[aiID].gameObject);
        aiList.RemoveAt(aiID);

        if(aiList.Count == 0)
        {
            StartCoroutine(Win());
            return;
        }

        for(int i = 0; i < aiList.Count; i++)
        {
            if(aiList[i].id != i)
            {
                aiList[i].id--;
            }
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
            StartCoroutine(Defeat());
            return;
        }

        switch (turnType)
        {
            case TurnType.Wait_Player:
                _uiManager.ViewText("Player Turn", () =>
                {
                    _turnCount = _maxTurnCount;
                    turnType = TurnType.Input_Action;
                });
                break;

            case TurnType.Player_Move:
            case TurnType.Player_Attack:
                turnType = TurnType.Wait_AI;
                TurnChange();
                break;

            case TurnType.Wait_AI:
                _aiMoveCount = aiList.Count;
                _uiManager.ViewText("Enemy Turn", () =>
                {
                    AITurn();
                });
                break;

            case TurnType.AI:
                _aiMoveCount--;
                if(_aiMoveCount == 0)
                {
                    turn--;
                    isIng = false;
                    StartCoroutine(PlayerTurn());
                }
                break;
        }
    }

    private void AITurn()
    {
        isIng = true;
        turnType = TurnType.AI;

        _aStar.PathFinding(); //��, ������Ʈ ������ ����

        foreach(var ai in aiList)
        {
            ai.AIMoveStart();
        }
    }

    private IEnumerator PlayerTurn()
    {
        yield return null;   
        turnType = TurnType.Wait_Player;

        foreach(var p in playerList)
        {
            p.isFight = false;
            p.isMove = false;
        }

        OnUIChange?.Invoke();
        TurnChange();
    }

    public void ClickTile(GameObject tileObj)
    {
        GameObject highLight = tileObj.transform.Find("_highlight").gameObject;
        Tile tile = tileObj.GetComponent<Tile>();

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
                if (tile.isPlayer())
                {
                    turnType = TurnType.Input_Action;
                    HideDistance();
                    return;
                }

                if (MoveCheck(tile) && DistanceCheck(tile.tile.Position))
                {
                    PlayerMove(tile.tile.Position, tileObj.transform);
                }
            }
            else if (turnType == TurnType.Player_Attack)
            {
                if (tile.isPlayer())
                {
                    turnType = TurnType.Input_Action;
                    HideDistance();
                }
                else if(tile.isAI())
                {
                    if (Vector2Int.Distance(tile.tile.Position, pPos) <= 1)
                        PlayerAttack(tileObj.GetComponentInChildren<AI>().id);
                }
            }
        }
    }

    public void ShowUpdateStat(Player _player)
    {
        _uiManager.ShowPlayerStatusUI(_player);
    }

    public void ShowUpdateStat(AI _ai)
    {
        _uiManager.ShowAIStatusUI(_ai);
    }

    public void TurnStop()
    {
        if(turnType == TurnType.Input_Action)
        {
            turnType = TurnType.Wait_AI;
            _uiManager.TurnstopEmphasisStop();
            TurnChange();
        }
    }

    public IEnumerator Win()
    {
        yield return new WaitForSeconds(0.5f);
        for(int y = 0; y < 8; y++)
        {
            for(int x = 0; x < 8; x++)
            {
                int slot = Mathf.FloorToInt((7 - y) * 8 + x);
                Destroy(tileList[slot].gameObject);
            }
            yield return new WaitForSeconds(0.06f);
        }
        _uiManager.ShowInfoUI(false);

        yield return new WaitForSeconds(0.2f);
        _uiManager.ViewText("Battle Command Complete", () =>
        {
            sendChatID = 4;
            SceneManager.LoadScene("Typing");
        });
    }

    public IEnumerator Defeat()
    {
        yield return new WaitForSeconds(0.5f);
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                int slot = Mathf.FloorToInt((7 - y) * 8 + x);
                Destroy(tileList[slot].gameObject);
            }
            yield return new WaitForSeconds(0.06f);
        }
        _uiManager.ShowInfoUI(false);

        yield return new WaitForSeconds(0.2f);
        _uiManager.ViewText("Battle Command Faild!", () => 
        {
            tileList.Clear();
            playerList.Clear();
            aiList.Clear();

            turn = maxTurn;

            StartCoroutine(TileSpawn(false));
            _uiManager.ShowInfoUI(true);
        });
    }
}
