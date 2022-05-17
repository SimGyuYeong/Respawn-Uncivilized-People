using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonExplaning : ButtonManager, IPointerEnterHandler, IPointerExitHandler
{
    private ExplanaingText _explanaingText;

    [SerializeField]
    private int buttonIntType;

    private void Awake()
    {
        _explanaingText = FindObjectOfType<ExplanaingText>();
    }
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("���ä�");
        _explanaingText.TestInterface(buttonIntType);
        switch (buttonIntType)
        {
            case 0:
                _explanaingText.ShowText("�ѱ��� ��Ÿ����� ���� �� �� �ִ� ī��. \n���� �ֱٿ� ������ �� ���̴���, ����� ����� ������ �Һ��ڵ��� ������ ���� �����.");
                break;
            case 1:
                _explanaingText.ShowText("3��¥�� �ǹ��� ���� ����ϴ� �Ŵ��� �Ը��� �йи� �������...\nó�� ��������, �� �Ը�ʹ� ��︮�� �ʴ� '�ھ��� ������' �̶�� ������ �λ����̴�.");
                break;
            case 2:
                _explanaingText.ShowText("�ٴ��� �췹ź���� ����Ǿ��ִ�, ��̵��� ���� ������ ������. \n�� �ǳ����� ������ ��å ������ ������ ��ġ�ϰ� �ִ�.");
                break;
            case 3:
                _explanaingText.ShowText("��¦�Ÿ��� ����â���� �ڵ��� ���� Ÿ��. \nKBT�� ���� ���ѹα���ȭ���׷���å�����̴�");
                break;
        }
    }

    private void SetExText()
    {
        //GameObject currentBTN = EventSystem.current.currentSelectedGameObject;

        //string buttonName = currentBTN.name;

        //Debug.Log(buttonName);

        //switch(buttonName)
        //{
        //    case storyButton.:
        //        Debug.Log("�ȳ�");
        //        break;
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _explanaingText.CloseText();
    }

    //IEnumerator TextFadeIn()
    //{
    //    Color alpha = new Color(0, 64, 0);

    //    while (alpha.a <= 1)
    //    {
    //        alpha.a += 0.04f;
    //        explaningImage.color = alpha;
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //}

    //IEnumerator AddScale()
    //{
    //    explaningText.transform.DOScale(1f, 0.4f);
    //    yield return new WaitForSeconds(0.5f);
    //}
    //IEnumerator TextFadeOut()
    //{
    //    Color alpha = new Color(0, 64, 0);

    //    while (alpha.a >= 0)
    //    {
    //        alpha.a += 0.04f;
    //        explaningImage.color = alpha;
    //        yield return new WaitForSeconds(0.01f);
    //    }
    //}

    //IEnumerator MinScale()
    //{
    //    explaningText.transform.DOScale(0.1f, 0.4f);
    //    yield return new WaitForSeconds(0.5f);
    //}
}
