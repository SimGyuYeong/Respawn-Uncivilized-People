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

    private void Start()
    {
        gay.value = 3;
        textPanel.DOFade(textalpha, 0.3f);
    }
    bool isSeq = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isSeq)
        {
            TweenCallback t = () => isSeq = false;                         //함수를 변수 안에다가 넣어준다. <<-TweenCallback

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

    public void storebutton()
    {
        storePanel.transform.DOMove(Vector3.zero, 0.4f);
        storeColor.DOFade(1, 0.3f);
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
