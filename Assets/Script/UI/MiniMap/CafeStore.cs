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
    public Image textPanel;
    float textalpha = 0;
    public static bool textbool = false;
    CafeTextOutput cafeTextOutput;
    private MinimapButtonType BTNtypeManager;

    int namecode = 0;
    int textcode = 0;

    private void Start()
    {
        gay.value = 3;
        textPanel.DOFade(textalpha, 0.3f);
        cafeTextOutput = GetComponent<CafeTextOutput>();
    }

    bool isSeq = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isSeq)
        {
            TweenCallback t = () => isSeq = false;                     //함수를 변수 안에다가 넣어준다. <<-TweenCallback
            textalpha = 0;
            isSeq = true;
            Sequence seq = DOTween.Sequence();
            seq.Append(textPanel.DOFade(textalpha, 0.3f));
            seq.AppendInterval(.6f);
            seq.Append(storePanel.transform.DOMoveX(-20, 0.4f));
            seq.Join(storeColor.DOFade(0, 0.3f));                      //<<- Join은 Append랑 같이 실행하게 해준다.
            seq.AppendCallback(t);
            
        }
    }

    public void SetCafeEnum()
    {
        BTNtypeManager = MinimapButtonType.CAFE;
        storebutton();
    }
    public void SetDepartmentEnum()
    {
        BTNtypeManager = MinimapButtonType.DEPARTMENTSTORE;
        storebutton();
    }
    public void SetRestarantEnum()
    {
        BTNtypeManager = MinimapButtonType.RESTARANT;
        storebutton();
    }
    public void SetParkEnum()
    {
        BTNtypeManager = MinimapButtonType.PARK;
        storebutton();
    }

    public void storebutton()
    {
        switch(BTNtypeManager)
        {
            case MinimapButtonType.CAFE:
                namecode = 0;
                textcode = 0;
                break;
            case MinimapButtonType.DEPARTMENTSTORE:
                namecode = 1;
                textcode = 1;
                break;
            case MinimapButtonType.RESTARANT:
                namecode = 2;
                textcode = 2;
                break;
            case MinimapButtonType.PARK:
                namecode = 3;
                textcode = 3;
                break;
        }

        storePanel.transform.DOMove(Vector3.zero, 0.4f);
        storeColor.DOFade(1, 0.3f);
        //cafeTextOutput.TextLoad(namecode, textcode);
        gay.value--;
        Invoke("TextPanelOn", .6f);
    }

    void TextPanelOn()
    {
        Sequence seq = DOTween.Sequence();
        textalpha = 0.5f;
        textPanel.DOFade(textalpha, 0.3f);
        seq.AppendInterval(1f);
        seq.AppendCallback(() =>  textbool = true);
    }
}
