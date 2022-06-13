using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Memorial : MonoBehaviour
{
    private Text text;

    public MemorialSO data;



    private void Start()
    {
        text = GetComponentInChildren<Text>();
    }
}
