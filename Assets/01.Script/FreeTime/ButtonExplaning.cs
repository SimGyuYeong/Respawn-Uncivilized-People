using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonExplaning : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ExplanaingText _explanaingText;
    private FreeTimeText _freeTimeText = null;

    [SerializeField]
    private int buttonIntType;

    private void Awake()
    {
        _explanaingText = FindObjectOfType<ExplanaingText>();
        _freeTimeText = FindObjectOfType<FreeTimeText>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        _explanaingText.TestInterface(buttonIntType);
        if (!_freeTimeText._istuto)
        {
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
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        _explanaingText.CloseText();
    }
}
