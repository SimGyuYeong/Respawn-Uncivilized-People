using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightManager : MonoBehaviour
{
    public static FightManager Instance;
    public AStarAlgorithm _AStar;

    public List<Vector2Int> enemyPos = new List<Vector2Int>();
    public List<int> noneMoveEnemy = new List<int>();
    public List<Node> finalNodeList
    {
        get => _AStar.FinalNodeList;
    }
    public Vector2Int pPos
    {
        get => _AStar.playerPos;
        set => _AStar.playerPos = value;
    }
    public Vector2Int tPos
    {
        get => _AStar.targetPos;
        set => _AStar.targetPos = value;
    }

    public GameObject Content, Player, moveAni;
    public Tile TilePrefab;
    public AI Enemy;
    public bool move = false;
    public bool[] isWallList;

    public List<Tile> tileList = new List<Tile>();
    private List<AI> _aiList = new List<AI>();

    [SerializeField] Text turnText;

    [SerializeField] GameObject TileUI;
    [SerializeField] GameObject GoalUI;

    private int _energy = 100;
    public int Energy
    {
        get => _energy;
        set
        {
            _energy = Mathf.Clamp(value, 0, 100);
        }
    }

    private int _enemyCount = 3;
    [SerializeField] int turn = 10;

    public bool isClickPlayer = false;
    public int moveDistance = 4;

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
        _AStar = GetComponent<AStarAlgorithm>();
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

                if (_AStar.playerPos.x == x && _AStar.playerPos.y == y)
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
        tPos = new Vector2Int((int)pos.x, (int)pos.y);
        _AStar.PathFinding();
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

        if (_AStar.NodeArray[(int)pos.x, (int)pos.y].isWall)
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
        for (int i = 0; i < finalNodeList.Count; i++)
        {
            Vector2 pos = new Vector2(finalNodeList[i].x, finalNodeList[i].y);
            _lineRenderer.positionCount++;
            _lineRenderer.SetPosition(i, pos);
        }
    }

    public void EnemyDraw()
    {
        _lineRenderer.positionCount = 2;
        _lineRenderer.startColor = Color.red;
        _lineRenderer.endColor = Color.red;

        Vector2Int pos = pPos;
        _lineRenderer.SetPosition(0, new Vector3(pos.x, pos.y));
        pos = tPos;
        _lineRenderer.SetPosition(1, new Vector3(pos.x, pos.y));
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
        for(int i = 0; i < finalNodeList.Count; i++)
        {
            var obj = Instantiate(moveAni);
            obj.transform.position = new Vector2(finalNodeList[i].x, finalNodeList[i].y);
            _x = finalNodeList[i].x;
            _y = finalNodeList[i].y;
            Energy -= 2;
            yield return new WaitForSeconds(0.2f);
        }
        Player.transform.position = new Vector2(_x, _y);
        Player.SetActive(true);
        UpdateUI();
        yield return new WaitForSeconds(0.8f);
        pPos = new Vector2Int(_x, _y);
        
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
        tPos = pos;
        _AStar.PathFinding();
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
        while (value > Energy)
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
        while (value != _enemyCount)
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
