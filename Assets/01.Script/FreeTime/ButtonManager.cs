using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] protected UnityEngine.UI.Button[] storyButton;        //건물 버튼
    [SerializeField] protected Image fadeImage;                            //검은 화면
    [SerializeField] protected GameObject GameScreen;                      //게임 배경화면
    [SerializeField] protected UnityEngine.UI.Button textPanel;            //텍스트 검은 배경
    [SerializeField] protected Image settingPanel;
}
