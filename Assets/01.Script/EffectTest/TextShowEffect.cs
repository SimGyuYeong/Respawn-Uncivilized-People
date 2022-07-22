using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TextShowEffect : MonoBehaviour
{
    public Image background;
    public Text text;

    private float effectSpd = 0.2f;

    private void Start()
    {
        StartCoroutine(BackShow());
    }

    IEnumerator BackShow()
    {
        yield return new WaitForSeconds(0.5f);
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => StartCoroutine(TextShow()));
        seq.Append(background.gameObject.transform.DOScale(Vector3.one * 1.5f, 0));
        seq.Append(background.DOFade(1, effectSpd));
        seq.Join(background.gameObject.transform.DOScale(Vector3.one, effectSpd));
    }

    IEnumerator TextShow()
    {
        yield return new WaitForSeconds(0.1f);
        Sequence seq = DOTween.Sequence();
        seq.Append(text.gameObject.transform.DOScale(Vector3.one * 1.3f, 0));
        seq.Append(text.DOFade(1, effectSpd));
        seq.Join(text.gameObject.transform.DOScale(Vector3.one, effectSpd));
        seq.AppendInterval(1f);

        //사라지는거
        seq.AppendCallback(() => StartCoroutine(BackHide()));
        seq.Append(text.gameObject.transform.DOScale(Vector3.one * 1.3f, effectSpd));
        seq.Join(text.DOFade(0, effectSpd));
        
    }

    IEnumerator BackHide()
    {
        yield return new WaitForSeconds(0.1f);
        background.gameObject.transform.DOScale(Vector3.one * 1.3f, effectSpd);
        background.DOFade(0, effectSpd);
    }
}
