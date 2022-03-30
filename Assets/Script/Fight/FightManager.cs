using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _y)
    {
        isWall = _isWall;
        x = _x;
        y = _y;
    }

    // G: ���ۺ��� �̵��ߴ� �Ÿ�, H: |����|+|����| ��ǥ�������� �Ÿ�, F: G+H
    public bool isWall;
    public Node ParentNode;

    public int x, y, G, H;
    public int F { get { return G + H; } }
}

public class FightManager : MonoBehaviour
{
    public static FightManager Instance;

    #region A* �˰���
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

    public GameObject Content, Player, Enemy;
    public Tile TilePrefab;
    public GameObject moveAni;
    public bool move = false;
    public bool[] isWallList;

    private List<Tile> _tileList = new List<Tile>();

    public bool isClickPlayer = false;
    public int distance = 2;

    public LineRenderer _lineRenderer;

    private void Awake()
    {
        Instance = this;
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        StartCoroutine(spawnTile());

        _lineRenderer.startWidth = .05f;
        _lineRenderer.endWidth = .05f;
    }

    IEnumerator spawnTile()
    {
        int count = 1;
        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                var spawnedTile = Instantiate(TilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.transform.parent = Content.transform;
                spawnedTile.name = $"Tile {count}";
                spawnedTile.tile = new TileInform(count, x, y, false);

                _tileList.Add(spawnedTile);

                if (playerPos.x == x && playerPos.y == y)
                {
                    Player = Instantiate(Player, spawnedTile.transform);
                    Player.transform.position = spawnedTile.transform.position;
                }

                foreach(Vector2Int pos in enemyPos)
                {
                    if(pos == new Vector2Int(x, y))
                    {
                        var enemy = Instantiate(Enemy, spawnedTile.transform);
                        enemy.transform.position = spawnedTile.transform.position;
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

    #region A* �˰���
    public void PathFinding()
    {
        sizeX = maxPos.x - minPos.x + 1;
        sizeY = maxPos.y - minPos.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        //Node ����Ʈ�� Ÿ������ �Է�
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
                /* foreach (Collider2D colider in Physics2D.OverlapCircleAll(new Vector2(i, j), 0.4f))
                    if (colider.gameObject.layer == LayerMask.NameToLayer("wall")) //���̾ ���̸�
                        isWall = true; //�� True */

                NodeArray[i, j] = new Node(isWall, i, j); // ������, x��ǥ, y��ǥ
            }
        }

        // ���� �� ��Ʈ, ����Ʈ�� �ʱ�ȭ
        StartNode = NodeArray[playerPos.x, playerPos.y]; //���۳�� ����
        TargetNode = NodeArray[targetPos.x, targetPos.y]; //����� ����

        OpenList = new List<Node>() { StartNode }; //��ŸƮ��忡�� �����ϴϱ�, ���¸���Ʈ�� �ʱ�ȭ
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

            //������ Ÿ�Ϸ� ������
            if (CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;
                //������������ ��ǥ��������
                //��� ������� FinalNodeList�� �ֱ�
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                return;
            }

            // �� ������ �Ʒ� ����
            OpenListAdd(CurNode.x, CurNode.y + 1);
            OpenListAdd(CurNode.x + 1, CurNode.y);
            OpenListAdd(CurNode.x, CurNode.y - 1);
            OpenListAdd(CurNode.x - 1, CurNode.y);
        }
        
    }

    void OpenListAdd(int checkX, int checkY)
    {
        //�����ȿ� �ְ�, ���� �ƴϰ�, ���� ����Ʈ�� ���ٸ�
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

    public void ShowMoveDistance(bool view)
    {
        List<Vector2Int> _enemyPosList = enemyPos;

        int count = 0;

        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                if (!isWallList[count])
                {
                    if (DistanceCheck(new Vector2(x, y)))
                    {
                        SpriteRenderer _spriteRenderer = _tileList[count].GetComponent<SpriteRenderer>();
                        if (view)
                        {
                            bool enemy = false;
                            foreach(Vector2Int vec in _enemyPosList)
                            {
                                if (vec == new Vector2Int(x, y))
                                {
                                    enemy = true;
                                    _enemyPosList.Remove(vec);
                                }
                            }

                            if (enemy)
                                _spriteRenderer.color = Color.red;
                            else
                                _spriteRenderer.color = Color.yellow;
                        }
                            
                        else
                            _spriteRenderer.color = Color.white;
                    }
                }
                count++;
            }
        }
    }

    /// <summary>
    /// �̵����� �Ÿ�üũ
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool DistanceCheck(Vector2 pos)
    {
        targetPos = new Vector2Int((int)pos.x, (int)pos.y);
        PathFinding();
        if (FinalNodeList.Count <= distance+1) return true;
        else return false;
    }

    public void DrawLine()
    {
        _lineRenderer.positionCount = 0;
        for (int i = 0; i < FinalNodeList.Count; i++)
        {
            Vector2 pos = new Vector2(FinalNodeList[i].x, FinalNodeList[i].y);
            _lineRenderer.positionCount++;
            _lineRenderer.SetPosition(i, pos);
        }
    }

    /// <summary>
    /// �÷��̾� �̵� �ڷ�ƾ
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

            yield return new WaitForSeconds(0.2f);
        }
        Player.transform.position = new Vector2(_x, _y);
        Player.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        playerPos.x = _x;
        playerPos.y = _y;
        move = false;
        
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
}
