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

    public Vector2Int minPos, maxPos, playerPos, targetPos;
    public List<Node> FinalNodeList = new List<Node>();

    int sizeX, sizeY, i;
    Node[,] NodeArray;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;

    public GameObject Content;
    public Tile TilePrefab;
    public GameObject moveAni;
    public bool move = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PathFinding();
        StartCoroutine(spawnTile());
    }

    IEnumerator spawnTile()
    {
        int count = 1;
        int y = 7;
        for (int i = 0; i < 8; i++)
        {
            int x = 0;
            for (int j = 0; j < 8; j++)
            {
                var spawnedTile = Instantiate(TilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.transform.parent = Content.transform;
                spawnedTile.name = $"Tile {count}";
                spawnedTile.tile = new TileInform(count, x, y, false);

                //tileList.Add(spawnedTile.gameObject);

                //if (count == PlayerSpawnTN)
                //{
                //    Player = Instantiate(Player, spawnedTile.transform);
                //    Player.transform.position = spawnedTile.transform.position;
                //}

                //foreach (int value in EnemySpawnTN)
                //{
                //    if (value == count)
                //    {
                //        var sEnemy = Instantiate(Enemy, spawnedTile.transform);
                //        sEnemy.transform.position = spawnedTile.transform.position;
                //    }
                //}

                //if (tileActiveList[count - 1] == true)
                //{
                //    spawnedTile.notActive();
                //    spawnedTile.GetComponent<SpriteRenderer>().color = Color.gray;
                //}

                count++;
                if (i == 0) yield return new WaitForSeconds(0.08f);
                x++;
            }
            yield return new WaitForSeconds(0.08f);
            y--;
        }
    }

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
                foreach (Collider2D colider in Physics2D.OverlapCircleAll(new Vector2(i, j), 0.4f))
                    if (colider.gameObject.layer == LayerMask.NameToLayer("wall")) //���̾ ���̸�
                        isWall = true; //�� True

                NodeArray[i, j] = new Node(isWall, i, j); // ������, x��ǥ, y��ǥ
            }
        }

        // ���� �� ��Ʈ, ����Ʈ�� �ʱ�ȭ
        StartNode = NodeArray[playerPos.x, playerPos.y]; //���۳�� ����
        TargetNode = NodeArray[targetPos.x, targetPos.y]; //����� ����

        OpenList = new List<Node>() { StartNode }; //��ŸƮ��忡�� �����ϴϱ�, ���¸���Ʈ�� �ʱ�ȭ
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        while(OpenList.Count > 0)
        {
           
            CurNode = OpenList[0];
            for (i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H)
                    CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            //������ Ÿ�Ϸ� ������
            if(CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;
                //������������ ��ǥ��������
                //��� ������� FinalNodeList�� �ֱ�
                while(TargetCurNode != StartNode)
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

            if(MoveConst < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveConst;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (FinalNodeList.Count != 0) //���ӽ����� Draw �Ǵ°� ����
        {
            for (int i = 0; i < FinalNodeList.Count - 1; i++)
            {
                Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
                
            }
        }
    }

    public IEnumerator movePlayer()
    {
        int _x = 0, _Y = 0;
        for(int i = 0; i < FinalNodeList.Count; i++)
        {
            var obj = Instantiate(moveAni);
            obj.transform.position = new Vector2(FinalNodeList[i].x, FinalNodeList[i].y);
            _x = FinalNodeList[i].x;
            _Y = FinalNodeList[i].y;
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.8f);
        playerPos.x = _x;
        playerPos.y = _Y;
        move = false;
    }
}
