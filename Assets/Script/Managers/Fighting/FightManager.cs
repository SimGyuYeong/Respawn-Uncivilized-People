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

    // S: ���ۺ��� �̵��ߴ� �Ÿ�, T: |����|+|����| ��ǥ�������� �Ÿ�, A: S+T
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
        StartNode = NodeArray[StartNode.x, StartNode.y]; //���۳�� ����
        TargetNode = NodeArray[TargetNode.x, TargetNode.y]; //����� ����

        OpenList = new List<Node>() { StartNode }; //��ŸƮ��忡�� �����ϴϱ�, ���¸���Ʈ�� �ʱ�ȭ
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

                for (i = 0; i < FinalNodeList.Count; i++)
                    Debug.Log($"{i}��°�� {FinalNodeList[i].x}, {FinalNodeList[i].y}");
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
        if(checkX >= 0 && checkX < sizeX && checkY >= 0 && checkY < sizeY && !NodeArray[checkX, checkY].isWall && !ClosedList.Contains(NodeArray[checkX, checkY]))
        {

        }
    }

    private void OnDrawGizmos()
    {
        if (FinalNodeList.Count != 0) //���ӽ����� Draw �Ǵ°� ����
            for (int i = 0; i < FinalNodeList.Count - 1; i++)
                Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
    }
}
