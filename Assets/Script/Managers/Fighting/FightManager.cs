using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _y)
    {
        isWall = _isWall;
        
    }

    // S: 시작부터 이동했던 거리, T: |가로|+|세로| 목표지점까지 거리, A: S+T
    public bool isWall;
    public Node ParentNode;

    public int x, y, S, T;
    public int A { get { return S + T; } }
}

public class FightManager : MonoBehaviour
{
    public Vector2Int minPos, maxPos, playerPos, enemyPos;
    public List<Node> FinalNodeList;

    int sizeX, sizeY, i;
    Node[,] NodeArray;
    Node StartNode, TargetNode, CurNode;
    List<Node> OpenList, ClosedList;

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
                foreach (Collider2D colider in Physics2D.OverlapCircleAll(new Vector2(i, j), 0.4f))
                    if (colider.gameObject.layer == LayerMask.NameToLayer("wall")) //레이어가 벽이면
                        isWall = true; //벽 True

                NodeArray[i, j] = new Node(isWall, i, j); // 벽여부, x좌표, y좌표
            }
        }

        // 시작 끝 노트, 리스트들 초기화
        StartNode = NodeArray[StartNode.x, StartNode.y]; //시작노드 정보
        TargetNode = NodeArray[TargetNode.x, TargetNode.y]; //끝노드 정보

        OpenList = new List<Node>() { StartNode }; //스타트노드에서 시작하니까, 오픈리스트에 초기화
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        while(OpenList.Count > 0)
        {
            CurNode = OpenList[0];
            for (i = 1; i < OpenList.Count; i++)
                if (OpenList[i].A <= CurNode.A && OpenList[i].T < CurNode.T)
                    CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);

            //지정한 타일로 왔을때
            if(CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;
                //시작지점부터 목표지점까지
                //경로 순서대로 FinalNodeList에 넣기
                while(TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                for (i = 0; i < FinalNodeList.Count; i++)
                    Debug.Log($"{i}번째는 {FinalNodeList[i].x}, {FinalNodeList[i].y}");
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
        if(checkX >= 0 && checkX < sizeX && checkY >= 0 && checkY < sizeY && !NodeArray[checkX, checkY].isWall && !ClosedList.Contains(NodeArray[checkX, checkY]))
        {

        }
    }

    private void OnDrawGizmos()
    {
        if (FinalNodeList.Count != 0) //게임시작전 Draw 되는거 방지
            for (int i = 0; i < FinalNodeList.Count - 1; i++)
                Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
    }
}
