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

    // G: 시작부터 이동했던 거리, H: |가로|+|세로| 목표지점까지 거리, F: G+H
    public bool isWall;
    public Node ParentNode;

    public int x, y, G, H;
    public int F { get { return G + H; } }
}

public class AStarAlgorithm : MonoBehaviour
{
    public Vector2Int playerPos, targetPos, maxPos, minPos;
    private int i;

    public List<Node> FinalNodeList = new List<Node>();

    private int sizeX, sizeY;
    public Node[,] NodeArray;
    private Node StartNode, TargetNode, CurNode;
    private List<Node> OpenList, ClosedList;

    public void PathFinding()
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
                if (FightManager.Instance.isWallList[slot] == true)
                {
                    isWall = true;
                }
                foreach (Vector2 pos in FightManager.Instance.enemyPos)
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
}
