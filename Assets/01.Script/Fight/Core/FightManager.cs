using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

public class FightManager : MonoBehaviour
{
    //싱글톤
    public static FightManager Instance;

    #region A* 알고리즘
    
    private AStarAlgorithm _aStar; // A* 알고리즘 캐싱

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

    public List<Vector2Int> enemyPos = new List<Vector2Int>(); //적들의 좌표 리스트
    public List<Vector2Int> playerPos = new List<Vector2Int>(); //플레이어 부대들의 좌표 리스트
    public List<int> noneMoveEnemy = new List<int>(); //움직이지 않은 적들 리스트

    //타일이 생성될 오브젝트 부모, 플레이어 프리팹, 플레이어가 움직이는 애니메이션 오브젝트
    public GameObject content, player, moveAni;

    //타일 프리팹
    public Tile tilePrefab;

    //적 프리팹
    public AI enemyPrefab;

    //현재 플레이어 애니메이션이 작동중인지
    public bool isIng = false;

    //벽 정보 리스트
    public bool[] isWallList;
    
    //플레이어가 한턴에 몇번 행동할 수 있는지
    private int _turnPlayerCount = 2;

    //타일 리스트
    public List<Tile> tileList = new List<Tile>();

    //적 리스트
    public List<AI> enemyList = new List<AI>();

    

    //나중에 플레이어 스크립트 옮길 예정
    //움직였는지, 싸웠는지 체크하는 변수
    public bool move = false, fight = false;
    private int _energy = 100;

    //액션 버튼 오브젝트
    [SerializeField] private GameObject _actionButton;

    public int Energy
    {
        get => _energy;
        set
        {
            _energy = Mathf.Clamp(value, 0, 100);
            if(_energy==0)
            {
                Debug.Log("게임종료");
            }
        }
    }

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

    /// <summary>
    /// 타일 생성 코루틴 함수
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

                foreach(Vector2Int pos in playerPos)
                {
                    if (pos == new Vector2Int(x, y))
                    {
                        player = Instantiate(player, spawnedTile.transform);
                        player.transform.position = spawnedTile.transform.position;
                        pPos = pos;
                    }
                }

                foreach(Vector2Int pos in enemyPos)
                {
                    if(pos == new Vector2Int(x, y))
                    {
                        AI enemy = Instantiate(enemyPrefab, spawnedTile.transform);
                        enemy.transform.position = spawnedTile.transform.position;
                        spawnedTile.tile.isEnemy = true;
                        int _c = count;
                        enemy.ai = new AIInform(aiCount, x, y, 45, --_c);
                        enemyList.Add(enemy);
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

        foreach(var p in enemyPos)
        {
            if (p == pos)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 공격이 가능한지 체크하는 함수
    /// </summary>
    /// <returns></returns>
    private bool AttackCheck()
    {
        if (fight == false)
        {
            foreach (Vector2 pos in enemyPos)
            {
                if (Vector2.Distance(pos, pPos) <= 1) return true;
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
        move = true;
        tPos = pos;
        _aStar.PathFinding();
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
        player.SetActive(false);

        for(int i = 0; i < _finalNodeList.Count; i++)
        {
            var obj = Instantiate(moveAni);
            obj.transform.position = new Vector2(_finalNodeList[i].x, _finalNodeList[i].y);
            Energy -= 2;
            yield return new WaitForSeconds(0.2f);
        }
        Energy += 2;
        player.transform.position = new Vector3(tPos.x, tPos.y);
        player.SetActive(true);
        OnUIChange?.Invoke();
        yield return new WaitForSeconds(2f);
        pPos = tPos;
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
            if(action == Action.Move)
            {
                move = true;
                if (AttackCheck())
                {
                    turnType = TurnType.Input_Action;
                    return;
                }
            }
            else if (action == Action.Attack)
            {
                fight = true;
                if (move == false)
                {
                    turnType = TurnType.Input_Action;
                    return;
                }
            }
        }

        TurnChange();
    }

    /// <summary>
    /// 플레이어를 클릭했을 때
    /// </summary>
    public void ClickPlayer()
    {
        isClickPlayer = !isClickPlayer;
        if(!isClickPlayer)
        {
            lineRenderer.positionCount = 0;
            _actionButton.SetActive(false);
            HideDistance();
        }
        else
        {
            _actionButton.transform.position = new Vector3(pPos.x, pPos.y);
            _actionButton.SetActive(true);

            TextMeshProUGUI moveText = _actionButton.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI fightText = _actionButton.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            
            Image image = _actionButton.transform.GetChild(0).GetComponent<Image>();
            Color c = image.color;
            if(move)
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

            image = _actionButton.transform.GetChild(1).GetComponent<Image>();
            c = image.color;
            if (fight || AttackCheck() == false)
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
    }

    public void PlayerAction(Action value)
    {
        if (value == Action.Attack && AttackCheck() == false)
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
        Energy -= enemyList[enemyCount].ai.Health;
        enemyPos.RemoveAt(enemyCount);
        
        if(noneMoveEnemy.Count > 0)
        {
            noneMoveEnemy.RemoveAt(enemyCount);
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
            Debug.Log("게임종료");
            return;
        }

        switch (turnType)
        {
            case TurnType.Wait_Player:
                Debug.Log("Player Turn");
                turnType = TurnType.Input_Action;
                _turnPlayerCount = playerPos.Count*2;
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
                if(noneMoveEnemy.Count <= 0)
                {
                    //모든 적들을 움직이지 않는 적 리스트에 다시 넣는다.
                    for (int i = 0; i < enemyPos.Count; i++)
                        noneMoveEnemy.Add(i);
                }

                int _num = Random.Range(0, noneMoveEnemy.Count);
                enemyList[noneMoveEnemy[_num]].AIMoveStart(); //랜덤으로 지정한 적을 움직인다.
                noneMoveEnemy.RemoveAt(_num); //움직인 적은 움직이지 않은 적 리스트에서 제거
                if (noneMoveEnemy.Count < 1) //모든 적이 움직였다면
                    noneMoveEnemy.Clear(); //움직이지 않은 적 리스트 초기화

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
        move = false;
        fight = false;
        TurnChange();
    }

    
}
