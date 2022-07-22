using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public GameObject linePrefab;

    private LineRenderer _lineRenderer;
    private EdgeCollider2D _edgeCol;
    private List<Vector2> _point = new List<Vector2>();

    public void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _edgeCol = GetComponent<EdgeCollider2D>();
    }

    public void DrawLine()
    {

    }
}
