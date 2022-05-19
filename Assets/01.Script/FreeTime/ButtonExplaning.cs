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
                    _explanaingText.ShowText("한국의 길거리에서 흔히 볼 수 있는 카페. \n비교적 최근에 개업한 듯 보이는한, 깔끔한 모습의 매장이 소비자들의 눈길을 끌게 만든다.");
                    break;
                case 1:
                    _explanaingText.ShowText("3층짜리 건물을 전부 사용하는 거대한 규모의 패밀리 레스토랑...\n처럼 보이지만, 그 규모와는 어울리지 않는 '박씨네 국밥집' 이라는 간판이 인상적이다.");
                    break;
                case 2:
                    _explanaingText.ShowText("바닥이 우레탄으로 포장되어있는, 어린이들을 위한 안전한 놀이터. \n그 건너편에는 가볍게 산책 가능한 공원이 위치하고 있다.");
                    break;
                case 3:
                    _explanaingText.ShowText("반짝거리는 유리창으로 뒤덮인 높은 타워. \nKBT의 뜻은 대한민국생화학테러대책본부이다");
                    break;
            }
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        _explanaingText.CloseText();
    }
}
