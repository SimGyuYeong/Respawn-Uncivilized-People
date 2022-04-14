using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FightManager : MonoBehaviour
{
    //�̱���
    public static FightManager Instance;

    #region A* �˰���
    
    public AStarAlgorithm _AStar; // A* �˰��� ĳ��

    //A* �˰��򿡼� �������� ������
    public List<Node> finalNodeList { get => _AStar.FinalNodeList; }

    //A* �˰��� ��ϵ� �÷��̾� �δ��� ��ǥ�� �������ų� ����
    public Vector2Int pPos
    {
        get => _AStar.playerPos;
        set => _AStar.playerPos = value;
    }

    //A* �˰��� ��ϵ� Ÿ�� ��ǥ�� �������ų� ����
    public Vector2Int tPos
    {
        get => _AStar.targetPos;
        set => _AStar.targetPos = value;
    }
    #endregion

    public List<Vector2Int> enemyPos = new List<Vector2Int>(); //������ ��ǥ ����Ʈ
    public List<Vector2Int> playerPos = new List<Vector2Int>(); //�÷��̾� �δ���� ��ǥ ����Ʈ
    public List<int> noneMoveEnemy = new List<int>(); //�������� ���� ���� ����Ʈ

    //Ÿ���� ������ ������Ʈ �θ�, �÷��̾� ������, �÷��̾ �����̴� �ִϸ��̼� ������Ʈ
    public GameObject Content, Player, moveAni;

    //Ÿ�� ������
    public Tile TilePrefab;

    //�� ������
    public AI Enemy;

    //���� �÷��̾� �ִϸ��̼��� �۵�������
    public bool isIng = false;

    //�� ���� ����Ʈ
    public bool[] isWallList;
    
    //�÷��̾ ���Ͽ� ��� �ൿ�� �� �ִ���
    private int TurnPlayerCount = 2;

    //Ÿ�� ����Ʈ
    public List<Tile> tileList = new List<Tile>();

    //�� ����Ʈ
    private List<AI> _aiList = new List<AI>();

    [SerializeField] Text turnText;

    [SerializeField] GameObject TileUI;
    [SerializeField] GameObject GoalUI;

    private Text _energyText;

    //���߿� �÷��̾� ��ũ��Ʈ �ű� ����
    //����������, �ο����� üũ�ϴ� ����
    public bool move = false, fight = false;
    private int _energy = 100;
    public int Energy
    {
        get => _energy;
        set
        {
            _energy = Mathf.Clamp(value, 0, 100);
            if(_energy==0)
            {
                Debug.Log("��������");
            }
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
        _energyText = TileUI.transform.GetChild(1).GetComponent<Text>();
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
        TurnChange();
    }

    /// <summary>
    /// �̵� ���� �Ÿ� �����ִ� �Լ�
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
    /// �÷��̾� �̵� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    public IEnumerator movePlayer()
    {
        isIng = true;
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
        Energy += 2;
        Player.transform.position = new Vector2(_x, _y);
        Player.SetActive(true);
        UpdateUI();
        yield return new WaitForSeconds(2f);
        pPos = new Vector2Int(_x, _y);
        NextPlayerTurn();
    }

    /// <summary>
    /// �÷��̾� �ൿ �� �����Լ�
    /// </summary>
    private void NextPlayerTurn()
    {
        TurnPlayerCount--;
        isIng = false;
        if (TurnPlayerCount > 0)
        {
            if(!fight)
            {
                foreach (Vector2 pos in enemyPos)
                {
                    if (Vector2.Distance(pos, pPos) <= 1) return;
                }
            }        
        }

        TurnChange();
    }

    /// <summary>
    /// �÷��̾ Ŭ������ ��
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
    /// ĳ���� �̵� �Լ�
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
        UpdateEnergyUI();
    }

    private void UpdateEnergyUI()
    {
        TileUI.transform.GetChild(3).transform.DOScaleX((float)Energy / 100, 1.5f)
            .OnComplete(()=>scaleChange());
    }

    private void scaleChange()
    {
        _energyText.transform.DOShakeScale(0.4f, 0.7f, 5);
        _energyText.text = Energy.ToString();
    }

    public void TurnChange()
    {
        turn--;
        turnText.text = string.Format("������ {0}��", turn);

        if(turn==0)
        {
            Debug.Log("��������");
            return;
        }

        switch (turnType)
        {
            case TurnType.Wait_Player:
                Debug.Log("Player Turn");
                turnType = TurnType.Player;
                TurnPlayerCount = playerPos.Count*2;
                break;

            case TurnType.Player:
                turnType = TurnType.Wait_AI;
                TurnChange();
                break;

            case TurnType.Wait_AI:
                Debug.Log("AI Turn");
                turnType = TurnType.AI;

                //�������� �ʴ� ���� ���ٸ�
                if(noneMoveEnemy.Count <= 0)
                {
                    //��� ������ �������� �ʴ� �� ����Ʈ�� �ٽ� �ִ´�.
                    for (int i = 0; i < enemyPos.Count; i++)
                        noneMoveEnemy.Add(i);
                }

                int _num = Random.Range(0, noneMoveEnemy.Count);
                _aiList[noneMoveEnemy[_num]].AIMoveStart(); //�������� ������ ���� �����δ�.
                noneMoveEnemy.RemoveAt(_num); //������ ���� �������� ���� �� ����Ʈ���� ����
                if (noneMoveEnemy.Count < 1) //��� ���� �������ٸ�
                    noneMoveEnemy.Clear(); //�������� ���� �� ����Ʈ �ʱ�ȭ

                break;

            case TurnType.AI:
                UpdateUI();
                StartCoroutine(playerTurn());
                break;
        }
    }

    private IEnumerator playerTurn()
    {
        yield return new WaitForSeconds(2f);
        turnType = TurnType.Wait_Player;
        move = false;
        TurnChange();
    }

}
