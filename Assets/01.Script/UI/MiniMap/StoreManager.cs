using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class StoreManager : MonoBehaviour
{
    public GameObject storePanel = null;
    public Image storeColor;
    public Slider guage;
    public Image textPanel;
    public Text text;
    float textImageAlpha = 0;
    float textAlpha = 0;
    public static bool textbool = false;
    CafeTextOutput cafeTextOutput;
    ChangeImage changeImage;
    private MinimapButtonType BTNtypeManager;
    int namecode = 0;
    int textIndex = 0;
    private SpawnItemPanel spawnItemPanel;

    private void Start()
    {
        guage.value = 3;
        textPanel.DOFade(textImageAlpha, 0.3f);
        cafeTextOutput = GetComponent<CafeTextOutput>();
        changeImage = GetComponent<ChangeImage>();
        spawnItemPanel = GetComponent<SpawnItemPanel>();
    }

    bool isSeq = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isSeq)
        {
            TweenCallback t = () => isSeq = false;                     //함수를 변수 안에다가 넣어준다. <<-TweenCallback
            textImageAlpha = 0;
            textAlpha = 0;
            isSeq = true;
            Sequence seq = DOTween.Sequence();
            seq.Append(textPanel.DOFade(textImageAlpha, 0.3f));
            seq.Append(text.DOFade(textImageAlpha, 0.3f));
            seq.AppendInterval(.3f);
            seq.Append(storePanel.transform.DOMoveX(-20, 0.4f));
            seq.Join(storeColor.DOFade(0, 0.3f));                      //<<- Join은 Append랑 같이 실행하게 해준다.
            seq.AppendCallback(t);
            seq.Join(textPanel.transform.DOMoveY(-9, .3f));
            seq.Join(text.transform.DOMoveY(-9, .3f));
        }
    }

    public void OnBuilding(int SelectType)
    {
        switch(SelectType)
        {
            case (int)MinimapButtonType.CAFE:
                namecode = 0;
                textIndex = 0;
                break;
            case (int)MinimapButtonType.DEPARTMENTSTORE:
                namecode = 1;
                textIndex = 1;
                break;
            case (int)MinimapButtonType.RESTARANT:
                namecode = 2;
                textIndex = 2;
                break;
            case (int)MinimapButtonType.PARK:
                namecode = 3;
                textIndex = 3;
                break;
        }

        changeImage.ChangeStorePanel(SelectType);
        Storebutton(); 
    }

    public void Storebutton()
    {
        storePanel.transform.DOMove(Vector3.zero, 0.4f);
        storeColor.DOFade(1, 0.3f);
        //Debug.Log(namecode + " " + textIndex);
        //cafeTextOutput.TextLoad(namecode, textIndex);
        spawnItemPanel.SpawnPanel();
        guage.value--;
        Invoke("TextPanelOn", .6f);
    }

    void TextPanelOn()
    {
        Sequence seq = DOTween.Sequence();
        textImageAlpha = 0.5f;
        textAlpha = 1;
        
        textPanel.DOFade(textImageAlpha, 0.3f);
        text.DOFade(textAlpha, 0.3f);
        textPanel.transform.DOMoveY(-4, .3f);
        text.transform.DOMoveY(-4, .3f);

        seq.AppendInterval(1f);
        seq.AppendCallback(() =>  textbool = true);
    }
}
