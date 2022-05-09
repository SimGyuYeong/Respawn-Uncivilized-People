using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Text turnText;

    [SerializeField] GameObject tileUI;
    [SerializeField] GameObject goalUI;

    private Text _energyText;
    private Text _goalText;

    private void Awake()
    {
        _energyText = tileUI.transform.GetComponentInChildren<Text>();
        _goalText = goalUI.transform.GetComponentInChildren<Text>();
    }

    public void UpdateTurnText()
    {
        turnText.text = string.Format("앞으로 {0}턴", FightManager.Instance.turn);
    }

    public void UpdateGoalUI()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(goalUI.transform.GetChild(3).transform.DOScaleX((float)FightManager.Instance.enemyList.Count / 3, 1.5f));
        seq.Append(_goalText.transform.DOShakeScale(0.4f, 0.7f, 5));
        seq.AppendCallback(() =>
        {
            _goalText.text = FightManager.Instance.enemyList.Count.ToString();
        });
    }

    public void UpdateEnergyUI()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(tileUI.transform.GetChild(3).transform.DOScaleX((float)FightManager.Instance.Energy / 100, 1.5f));
        seq.Append(_energyText.transform.DOShakeScale(0.4f, 0.7f, 5));
        seq.AppendCallback(() =>
        {
            _energyText.text = FightManager.Instance.Energy.ToString();
        });

    }
}
