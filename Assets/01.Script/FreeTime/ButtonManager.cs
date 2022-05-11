using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] protected UnityEngine.UI.Button[] storyButton;        //�ǹ� ��ư
    [SerializeField] protected SpriteRenderer fadeImage;                            //���� ȭ��
    [SerializeField] protected GameObject GameScreen;                      //���� ���ȭ��
    [SerializeField] protected GameObject settingPanel;
}
