using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CreditScroll : MonoBehaviour
{
    private void OnEnable()
    {
        this.gameObject.transform.position = new Vector2(1000f, -1600f);
        this.gameObject.transform.DOKill();
        this.gameObject.transform.DOMoveY(1500f, 10f);
    }
}
