using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] protected UnityEngine.UI.Button[] storyButton;        //�ǹ� ��ư
    [SerializeField] protected Image fadeImage;                            //���� ȭ��
    [SerializeField] protected GameObject GameScreen;                      //���� ���ȭ��
    [SerializeField] protected UnityEngine.UI.Button textPanel;            //�ؽ�Ʈ ���� ���
    [SerializeField] protected Image settingPanel;
}
