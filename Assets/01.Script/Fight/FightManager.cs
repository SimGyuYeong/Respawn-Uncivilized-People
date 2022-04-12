using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _y)
    {
        isWall = _isWall;
        x = _x;
        y = _y;
    }

    // G: 시작부터 이동했던 거리, H: |가로|+|세로| 목표지점까지 거리, F: G+H
    public bool isWall;
    public Node ParentNode;

    public int x, y, G, H;
    public int F { get { return G + H; } }
}

public class FightManager : MonoBehaviour
{
    public static FightManager Instance;

    #region A* 알고리즘
    [Range(0f, 7f)]
    public Vector2Int playerPos, targetPos;
     
    public Vector2Int minPos, maxPos;
    private int i;

    private List<Node> FinalNodeList = new List<Node>();

    private int sizeX, sizeY;
    private Node[,] NodeArray;
    private Node StartNode, TargetNode, CurNode;
    private List<Node> OpenList, ClosedList;
    #endregion

    public List<Vector2Int> enemyPos = new List<Vector2Int>();
    public List<int> noneMoveEnemy = new List<int>();

    public GameObject Content, Player;
    public Tile TilePrefab;
    public AI Enemy;
    public GameObject moveAni;
    public bool move = false;
    public bool[] isWallList;

    public List<Tile> tileList = new List<Tile>();
    private List<AI> _aiList = new List<AI>();

    [SerializeField] Text turnText;

    [SerializeField] GameObject TileUI;
    [SerializeField] GameObject GoalUI;

    public int energy = 100;
    private int enemyCount = 3;
    [SerializeField] int turn = 10;

    public bool isClickPlayer = false;
    public int distance = 4;

    public LineRenderer _lineRenderer;

    public enum TurnType
    {
        Player,
        Wait_AI,
        AI,
        Wait_Player
    }
    public TurnType turnType = TurnType.Wait_Player;

    private void Awake()
    {
        Instance = this;
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        StartCoroutine(spawnTile());
        UpdateUI();

        _lineRenderer.startWidth = .05f;
        _lineRenderer.endWidth = .05f;
    }

    IEnumerator spawnTile()
    {
        int count = 1, aiCount = 0;
        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                var spawnedTile = Instantiate(TilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                spawnedTile.transform.parent = Content.transform;
                spawnedTile.name = $"Tile {count}";
                spawnedTile.tile = new TileInform(count, x, y, false, false);

                tileList.Add(spawnedTile);

                if (playerPos.x == x && playerPos.y == y)
                {
                    Player = Instantiate(Player, spawnedTile.transform);
                    Player.transform.position = spawnedTile.transform.position;
                }

                foreach(Vector2Int pos in enemyPos)
                {
                    if(pos == new Vector2Int(x, y))
                    {
                        AI enemy = Instantiate(Enemy, spawnedTile.transform);
                        enemy.transform.position = spawnedTile.transform.position;
                        spawnedTile.tile.isEnemy = true;
                        int _c = count;
                        enemy.ai = new AIInform(aiCount, x, y, 45, --_c);
                        _aiList.Add(enemy);
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
    }

    #region A* 알고리즘
    public void PathFinding(char type = 'd')
    {
        sizeX = maxPos.x - minPos.x + 1;
        sizeY = maxPos.y - minPos.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        //Node 리스트에 타일정보 입력
        for (i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                int slot = (56 + i) - j * 8;
                if (isWallList[slot] == true)
                {
                    isWall = true;
                }
                foreach (Vector2 pos in enemyPos)
                {
                    if (pos == new Vector2(i, j))
                    {
                        isWall = true;
                    }
                }
                NodeArray[i, j] = new Node(isWall, i, j); // 벽여부, x좌표, y좌표
            }
        }

        // 시작 끝 노트, 리스트들 초기화
        StartNode = NodeArray[playerPos.x, playerPos.y]; //시작노드 정보
        TargetNode = NodeArray[targetPos.x, targetPos.y]; //끝노드 정보

        OpenList = new List<Node>() { StartNode }; //스타트노드에서 시작하니까, 오픈리스트에 초기화
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        while (OpenList.Count > 0)
        {

            CurNode = OpenList[0];
            for (i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H)
                    CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            //지정한 타일로 왔을때
            if (CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;
                //시작지점부터 목표지점까지
                //경로 순서대로 FinalNodeList에 넣기
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                return;
            }

            // 위 오른쪽 아래 왼쪽
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
        
    }

    void OpenListAdd(int checkX, int checkY)
    {
        //범위안에 있고, 벽이 아니고, 닫힌 리스트에 없다면
        if (checkX >= 0 && checkX < sizeX && checkY >= 0 && checkY < sizeY && !NodeArray[checkX, checkY].isWall && !ClosedList.Contains(NodeArray[checkX, checkY]))
        {
            Node NeighborNode = NodeArray[checkX, checkY];
            int MoveConst = CurNode.G + (CurNode.x - checkX == 0 || CurNode.y - checkY == 0 ? 10 : 14);

            if (MoveConst < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveConst;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }
    #endregion

    /// <summary>
    /// 이동 가능 거리 보여주는 함수
    /// </summary>
    /// <param name="view"></param>
    public void ShowMoveDistance(bool view)
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

                    if (DistanceCheck(new Vector2(x, y)))
                    {
                        _spriteRenderer.color = view ? Color.yellow : Color.white;
                    }
                }
                count++;
            }
        }
    }

    public bool DistanceCheck(Vector2 pos)
    {
        targetPos = new Vector2Int((int)pos.x, (int)pos.y);
        PathFinding();
        if (FinalNodeList.Count <= distance+1 && FinalNodeList.Count > 0) return true;
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

        if (NodeArray[(int)pos.x, (int)pos.y].isWall)
            return true;

        foreach(var p in enemyPos)
        {
            if (p == pos)
                return true;
        }

        return false;
    }

    public void DrawLine()
    {
        _lineRenderer.startColor = Color.white;
        _lineRenderer.endColor = Color.white;
        _lineRenderer.positionCount = 0;
        for (int i = 0; i < FinalNodeList.Count; i++)
        {
            Vector2 pos = new Vector2(FinalNodeList[i].x, FinalNodeList[i].y);
            _lineRenderer.positionCount++;
            _lineRenderer.SetPosition(i, pos);
        }
    }

    public void EnemyDraw()
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;
        _lineRenderer.SetPosition(0, new Vector3(playerPos.x, playerPos.y, 0));
        _lineRenderer.SetPosition(1, new Vector3(targetPos.x, targetPos.y, 0));
    }

    /// <summary>
    /// 플레이어 이동 코루틴
    /// </summary>
    /// <returns></returns>
    public IEnumerator movePlayer()
    {
        _lineRenderer.positionCount = 0;
        Player.SetActive(false);

        int _x = 0, _y = 0;
        for(int i = 0; i < FinalNodeList.Count; i++)
        {
            var obj = Instantiate(moveAni);
            obj.transform.position = new Vector2(FinalNodeList[i].x, FinalNodeList[i].y);
            _x = FinalNodeList[i].x;
            _y = FinalNodeList[i].y;
            energy -= 2;
            yield return new WaitForSeconds(0.2f);
        }
        Player.transform.position = new Vector2(_x, _y);
        Player.SetActive(true);
        UpdateUI();
        yield return new WaitForSeconds(0.8f);
        playerPos.x = _x;
        playerPos.y = _y;
        
    }

    /// <summary>
    /// 플레이어를 클릭했을 때
    /// </summary>
    public void ClickPlayer()
    {
        isClickPlayer = !isClickPlayer;
        if(!isClickPlayer)
        {
            _lineRenderer.positionCount = 0;
        }
        ShowMoveDistance(isClickPlayer);
    }

    /// <summary>
    /// 캐릭터 이동 함수
    /// </summary>
    public void PlayerMove(Vector2Int pos, Transform parent)
    {
        move = true;
        ClickPlayer();
        targetPos = pos;
        PathFinding();
        Player.transform.SetParent(parent);
        StartCoroutine(movePlayer());
    }

    public void UpdateUI()
    {
        turnText.text = string.Format("앞으로 {0}턴", turn);
        StartCoroutine(SoftTileValueChange());
        StartCoroutine(SoftGoalValueChange());
    }

    private IEnumerator SoftTileValueChange()
    {
       
        int value = (int)TileUI.transform.GetChild(2).GetComponent<Slider>().value;
        Text energyText = TileUI.transform.GetChild(1).GetComponent<Text>();
        while (value > energy)
        {
            TileUI.transform.GetChild(2).GetComponent<Slider>().value--;
            value = (int)TileUI.transform.GetChild(2).GetComponent<Slider>().value;
            energyText.text = string.Format("{0} / 100", value);
            yield return new WaitForSeconds(0.2f);
        }

        TurnChange();
    }

    private IEnumerator SoftGoalValueChange()
    {
        float value = GoalUI.transform.GetChild(2).GetComponent<Slider>().value;
        Text countText = GoalUI.transform.GetChild(1).GetComponent<Text>();
        while (value != enemyCount)
        {
            GoalUI.transform.GetChild(2).GetComponent<Slider>().value--;
            value = GoalUI.transform.GetChild(2).GetComponent<Slider>().value;
            countText.text = string.Format("{0} / 3", value);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void TurnChange()
    {
        switch(turnType)
        {
            case TurnType.Wait_Player:
                Debug.Log("Player Turn");
                StartCoroutine(waitSecond(1f));
                turnType = TurnType.Player;
                break;

            case TurnType.Player:
                turnType = TurnType.Wait_AI;
                TurnChange();
                break;

            case TurnType.Wait_AI:
                Debug.Log("AI Turn");
                StartCoroutine(waitSecond(1f));
                turnType = TurnType.AI;

                //움직이지 않는 적이 없다면
                if(noneMoveEnemy.Count <= 0)
                {
                    //모든 적들을 움직이지 않는 적 리스트에 다시 넣는다.
                    for (int i = 0; i < enemyPos.Count; i++)
                        noneMoveEnemy.Add(i);
                }

                int _num = Random.Range(0, noneMoveEnemy.Count);
                _aiList[noneMoveEnemy[_num]].AIMoveStart(); //랜덤으로 지정한 적을 움직인다.
                noneMoveEnemy.RemoveAt(_num); //움직인 적은 움직이지 않은 적 리스트에서 제거
                if (noneMoveEnemy.Count < 1) //모든 적이 움직였다면
                    noneMoveEnemy.Clear(); //움직이지 않은 적 리스트 초기화

                break;

            case TurnType.AI:
                turnType = TurnType.Wait_Player;
                move = false;
                TurnChange();
                break;
        }
    }

    private IEnumerator waitSecond(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
