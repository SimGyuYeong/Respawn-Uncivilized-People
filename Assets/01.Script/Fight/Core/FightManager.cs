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
    //싱글톤
    public static FightManager Instance;

    private Tutorial _tuto;
    private UIManager _uiManager;
    public UIManager UI { get => _uiManager; }

    // 전투 전용 사운드 매니저
    private FightSoundManager _soundManager;
    public FightSoundManager SoundManager { get => _soundManager; }

    #region A* 알고리즘
    
    private AStarAlgorithm _aStar; // A* 알고리즘 캐싱

    public AStarAlgorithm AStar => _aStar;

    //A* 알고리즘에서 길정보를 가져옴
    public List<Node> finalNodeList { get => _aStar.FinalNodeList; }

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

    public List<PlayerData> playerDataList = new List<PlayerData>();
    public List<AIData> aiDataList = new List<AIData>();


    //타일이 생성될 오브젝트 부모, 플레이어 프리팹, 플레이어가 움직이는 애니메이션 오브젝트
    public GameObject content, moveAni;

    public Tile tilePrefab;
    public AI enemyPrefab;
    public Player playerPrefab;

    public Player player;

    //현재 플레이어 애니메이션이 작동중인지
    public bool isIng = false;

    //벽 정보 리스트
    public bool[] isWallList;
    
    //플레이어가 한턴에 몇번 행동할 수 있는지
    private int _maxTurnCount = 0;
    private int _turnCount = 0;

    public List<Tile> tileList = new List<Tile>();
    public List<AI> aiList = new List<AI>();
    public List<Player> playerList = new List<Player>();

    //액션 버튼 오브젝트
    [SerializeField] private GameObject _actionButton;

    public int maxTurn = 10;
    public int turn = 0;

    public bool isClickPlayer = false;
    public int moveDistance = 4;

    private int _pCount = 0;
    private int _aiMoveCount = 0;

    public static int sendChatID = 0;

    public LineRenderer lineRenderer;

    public bool isSkillSelect = false;

    [field: SerializeField] public UnityEvent OnUIChange { get; set; }

    public enum TurnType
    {
        AI_Wait = 0,
        AI,
        Player_Wait,
        Player,
        Player_Ing,
    }

    public TurnType turnType = TurnType.Player_Wait;

    public enum InputType
    { 
        None,
        Input_Move,
        Input_Skill,
    }

    public InputType pInput = InputType.None;

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
        _soundManager = transform.parent.Find("FightSoundManager").GetComponent<FightSoundManager>();
    }

    private void Start()
    {
        turnType = TurnType.Player_Wait;
        StartCoroutine(TileSpawn(false));
        //isIng = true;
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
    /// 타일 생성 코루틴 함수
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

    #region 라인 그리기
    public void DrawLine()
    {
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.positionCount = 0;
        for (int i = 0; i < finalNodeList.Count; i++)
        {
            Vector2 pos = new Vector2(finalNodeList[i].x, finalNodeList[i].y);
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(i, pos);
        }
    }

    public void EnemyDraw()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.yellow;

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
        if (finalNodeList.Count <= moveDistance+1 && finalNodeList.Count > 0) return true;
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

    /// <summary>
    /// 플레이어가 움직일 수 있는 상태인지 체크
    /// </summary>
    /// <returns>움직일 수 있다면 True, 아니면 False</returns>
    public bool MoveCheck(Tile tile)
    {
        if (pInput == InputType.Input_Move
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

    public void ShowDistance(int distance)
    {
        SpriteRenderer _spriteRenderer;

        foreach(var _tile in tileList)
        {
            bool isSkill = false;
            TileInform _tileInform = _tile.GetComponent<Tile>().tile;

            bool isDistance = false;

            if(_tileInform.isWall == false && _tile.transform.childCount < 2) isDistance = true;

            if (isDistance)
            {
                _spriteRenderer = _tile.GetComponent<SpriteRenderer>();
                _aStar.targetPos = _tileInform.Position;
                _aStar.PathFinding(isSkill);
                if (finalNodeList.Count <= distance + 1 && finalNodeList.Count > 0)
                {
                    _spriteRenderer.color = isSkill == true ? Color.red : Color.yellow;
                }
            }

        }
    }

    public bool WhetherUseSkill(int count)
    {
        return count != 0;
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

        for(int i = 0; i < finalNodeList.Count; i++)
        {
            var obj = Instantiate(moveAni);
            obj.transform.position = new Vector2(finalNodeList[i].x, finalNodeList[i].y);
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
    /// 플레이어 행동 후 진행함수
    /// </summary>
    public void NextPlayerTurn(Action action)
    {
        _turnCount--;
        isIng = false;
        if (_turnCount > 0)
        {
            if (action == Action.Move) player.isMove = true;  
            else if (action == Action.Attack)  player.isFight = true; 

            foreach(Player p in playerList)
            {
                if (p.isFight == false && p.KineticPoint >= 15)
                {
                    return;
                }
                if (p.isMove == false)
                {
                    return;
                }
            }
        }

        _uiManager.TurnstopEmphasis();
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
        if (player.isFight)
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
        if (value == Action.Attack  && player.isFight == true)
            return;

        if (value == Action.Move && player.isMove == true)
            return;

        _actionButton.SetActive(false);
        switch(value)
        {
            case Action.Move:
                ShowDistance(moveDistance);
                pInput = InputType.Input_Move;
                break;
            case Action.Attack:
                pInput = InputType.Input_Skill;
                _uiManager.ShowSkillUI(true);
                break;
            case Action.Stop:
                turnType = TurnType.Player_Ing;
                TurnChange();
                break;
            default:
                break;
        }
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
            case TurnType.Player_Wait:
                _uiManager.ViewText("Player Turn", () =>
                {
                    _turnCount = _maxTurnCount;
                    turnType = TurnType.Player;
                });
                break;

            case TurnType.Player_Ing:
                if (aiList.Count == 0) StartCoroutine(Win());

                turnType = TurnType.AI_Wait;
                pInput = InputType.None;
                TurnChange();
                break;

            case TurnType.AI_Wait:
                _actionButton.SetActive(false);
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
                    PlayerTurn();
                }
                break;
        }
    }

    private void AITurn()
    {
        isIng = true;
        turnType = TurnType.AI;

        _aStar.PathFinding(); //벽, 오브젝트 데이터 리셋

        foreach(var ai in aiList)
        {
            ai.AIMoveStart();
        }
    }

    /// <summary>
    /// AI턴이 끝나면 Player 턴으로 넘어갈때 사용되는 함수
    /// </summary>
    private void PlayerTurn()
    {
        turnType = TurnType.Player_Wait;
        if (playerList.Count == 0)
        {
            StartCoroutine(Defeat());
            return;
        }

        foreach (var p in playerList)
        {
            p.isFight = false;
            p.isMove = false;
            p.KineticPoint += 25;
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
            if (pInput == InputType.None)
            {
                if(tileObj.transform.childCount >= 2)
                {
                    if(tileObj.transform.GetComponentInChildren<Player>() != null)
                    {
                        ClickPlayer(tileObj.transform.GetChild(1).GetComponent<Player>());
                    }
                }
            }

            else if (pInput == InputType.Input_Move)
            {
                if (tile.isPlayer())
                {
                    pInput = InputType.None;
                    HideDistance();
                    return;
                }

                if (MoveCheck(tile) && DistanceCheck(tile.tile.Position))
                {
                    PlayerMove(tile.tile.Position, tileObj.transform);
                }
            }
            else if (pInput == InputType.Input_Skill)
            {
                if (tile.isPlayer())
                {
                    pInput = InputType.None;
                    _uiManager.ShowSkillUI(false);
                }
                else if(tile.isAI())
                {
                    if(tileObj.GetComponent<SpriteRenderer>().color == Color.red)
                    {
                        AI ai = tileObj.GetComponentInChildren<AI>();
                        UI.selectSkill.SelectAI(ai);
                    }
                }
            }
        }
    }

    public List<AI> IronFistAttackList()
    {
        List<AI> attackAIList = new List<AI>();
        foreach (var _ai in aiList)
        {
            if (Vector2.Distance(_ai.Position, player.Position) == 1)
                attackAIList.Add(_ai);
        }
        return attackAIList;
    }

    public void TurnStop()
    {
        if(turnType == TurnType.Player)
        {
            turnType = TurnType.Player_Ing;
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
