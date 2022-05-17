using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager Instance;

    public void Awake()
    {
        Instance = this;
    }

    [SerializeField] protected UnityEngine.UI.Button[] storyButton;        //�ǹ� ��ư
    [SerializeField] public SpriteRenderer fadeImage;                   //���� ȭ��
    [SerializeField] public Sprite[] gameScreenBackground;                 //���� ���ȭ��
    [SerializeField] protected GameObject settingPanel;
}
