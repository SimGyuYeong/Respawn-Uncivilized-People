using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class FightManager : MonoBehaviour
{
    //싱글톤
    public static FightManager Instance;

    #region A* 알고리즘
    
    private AStarAlgorithm _aStar; // A* 알고리즘 캐싱

    public AStarAlgorithm AStar => _aStar;

    //A* 알고리즘에서 길정보를 가져옴
    private List<Node> _finalNodeList { get => _aStar.FinalNodeList; }

    //A* 알고리즘에 등록된 플레이어 부대의 좌표를 가져오거나 설정
    public Vector2Int pPos
    {
        get => _aStar.playerPos;
        set => _aStar.playerPos = value;
    }

    //A* 알고리즘에 등록된 타켓 좌표를 가져오거나 설정
    public Vector2Int tPos
    {
        get => _aStar.targetPos;
        set => _aStar.targetPos = value;
    }
    #endregion

    public List<ObjData> playerDataList = new List<ObjData>();
    public List<ObjData> aiDataList = new List<ObjData>();

    private List<int> _noneEnemyList = new List<int>(); //움직이지 않은 적들 리스트


    //타일이 생성될 오브젝트 부모, 플레이어 프리팹, 플레이어가 움직이는 애니메이션 오브젝트
    public GameObject content, moveAni;

    public Tile tilePrefab;
    public AI enemyPrefab;
    public Player playerPrefab;

    public AI ai;
    public Player player;

    //현재 플레이어 애니메이션이 작동중인지
    public bool isIng = false;

    //벽 정보 리스트
    public bool[] isWallList;
    
    //플레이어가 한턴에 몇번 행동할 수 있는지
    private int _turnPlayerCount = 0;

    public List<Tile> tileList = new List<Tile>();
    public List<AI> aiList = new List<AI>();
    private List<Player> _playerList = new List<Player>();

    //액션 버튼 오브젝트
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
        int count = 0;
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

    private void AISpawn()
    {
        int count = 0;
        foreach(var ai in aiDataList)
        {
            int slot = Mathf.FloorToInt((7 - ai.DPos.y) * 8 + ai.DPos.x);

            var _ai = Instantiate(enemyPrefab, tileList[slot].transform);
            _ai.DataSet(count, ai);

            aiList.Add(_ai);
            count++;
        }
    }

    /// <summary>
    /// 타일 생성 코루틴 함수
    /// </summary>
    /// <returns></returns>
    private IEnumerator spawnTile()
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
                if (y == 7) yield return new WaitForSeconds(0.08f);
            }
            yield return new WaitForSeconds(0.08f);
        }
        PlayerSpawn();
        AISpawn();

        OnUIChange?.Invoke();
        TurnChange();
    }

    #region 라인 그리기
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
    /// 이동가능한 거리인지 체크하는 함수
    /// </summary>
    /// <param name="pos">이동할 좌표</param>
    /// <returns>이동가능하면 True, 불가능하면 False</returns>
    public bool DistanceCheck(Vector2 pos)
    {
        tPos = new Vector2Int((int)pos.x, (int)pos.y);
        _aStar.PathFinding();
        if (_finalNodeList.Count <= moveDistance+1 && _finalNodeList.Count > 0) return true;
        return false;
    
        
    }
     
    /// <summary>
    /// 지형지물 체크 함수
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>지형지물 여부 반환</returns>
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
    /// 공격이 가능한지 체크하는 함수
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
    /// 캐릭터 이동 함수
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
    /// 플레이어 이동 코루틴
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
    /// 플레이어 행동 후 진행함수
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
    /// 플레이어를 클릭했을 때
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
        player.Energy -= aiList[enemyCount].energy;
        
        if(_noneEnemyList.Count > 0)
        {
            _noneEnemyList.RemoveAt(enemyCount);
        }
        
        Destroy(aiList[enemyCount].gameObject);
        aiList.RemoveAt(enemyCount);

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
            Debug.Log("게임종료");
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

                //움직이지 않는 적이 없다면
                if(_noneEnemyList.Count <= 0)
                {
                    //모든 적들을 움직이지 않는 적 리스트에 다시 넣는다.
                    for (int i = 0; i < aiList.Count; i++)
                        _noneEnemyList.Add(i);
                }

                int _num = Random.Range(0, _noneEnemyList.Count);
                aiList[_noneEnemyList[_num]].AIMoveStart(); //랜덤으로 지정한 적을 움직인다.
                _noneEnemyList.RemoveAt(_num); //움직인 적은 움직이지 않은 적 리스트에서 제거
                if (_noneEnemyList.Count < 1) //모든 적이 움직였다면
                    _noneEnemyList.Clear(); //움직이지 않은 적 리스트 초기화

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

    /// <summary>
    /// 플레이어가 움직일 수 있는 상태인지 체크
    /// </summary>
    /// <returns>움직일 수 있다면 True, 아니면 False</returns>
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

    public void ShowUpdateStat(Player _player)
    {
        UIManager.Instance.ShowStatUI(_player.playerName, _player.Energy, _player.info, 1, _player.id);
    }

    public void ShowUpdateStat(AI _ai)
    {
        UIManager.Instance.ShowStatUI(_ai.aiName, _ai.energy, _ai.info, 2, _ai.id);
    }
}
