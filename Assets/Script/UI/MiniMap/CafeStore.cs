using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CafeStore : MonoBehaviour
{

    
    Tweener store;
    public GameObject storePanel = null;
    public GameObject itemImage;
    public Image storeColor;
    public Slider gay;

    private void Start()
    {
        gay.value = 3;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            storePanel.transform.DOMoveX(-20, 0.4f);
            storeColor.DOFade(0, 0.3f);
        }
    }

    public void storebutton()
    {
        storePanel.transform.DOMove(Vector3.zero, 0.4f);
        storeColor.DOFade(1, 0.3f);
        gay.value--;
    }
}
