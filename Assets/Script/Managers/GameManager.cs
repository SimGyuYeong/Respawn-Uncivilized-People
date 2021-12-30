using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextManager textManager = null;
    public TextManager TEXT { get { return textManager; } }

    [SerializeField] private DataManager dataManager = null;
    public DataManager DATA {  get { return dataManager; } }

    private void Awake()
    {
        textManager = GetComponent<TextManager>();
        dataManager = GetComponent<DataManager>();
    }
}
