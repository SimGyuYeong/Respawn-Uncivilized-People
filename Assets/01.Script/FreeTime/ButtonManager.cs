using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] protected Button[] storyButton;       //건물 버튼
    [SerializeField] protected Image fadeImage;             //검은 화면
    [SerializeField] protected GameObject GameScreen;             //게임 배경화면
}
