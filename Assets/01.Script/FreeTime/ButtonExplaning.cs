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
        Debug.Log("나ㅓㅓ");
        _explanaingText.TestInterface(buttonIntType);
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

    private void SetExText()
    {
        //GameObject currentBTN = EventSystem.current.currentSelectedGameObject;

        //string buttonName = currentBTN.name;

        //Debug.Log(buttonName);

        //switch(buttonName)
        //{
        //    case storyButton.:
        //        Debug.Log("안녕");
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
