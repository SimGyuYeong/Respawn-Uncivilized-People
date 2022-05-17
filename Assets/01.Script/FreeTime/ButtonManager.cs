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

    [SerializeField] protected UnityEngine.UI.Button[] storyButton;        //건물 버튼
    [SerializeField] public SpriteRenderer fadeImage;                   //검은 화면
    [SerializeField] public Sprite[] gameScreenBackground;                 //게임 배경화면
    [SerializeField] protected GameObject settingPanel;
}
