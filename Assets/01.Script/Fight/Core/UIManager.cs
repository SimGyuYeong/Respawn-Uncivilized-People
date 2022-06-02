using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Text turnText;

    public GameObject infoUI;
    [SerializeField] GameObject goalUI;

    [SerializeField]
    private GameObject _playerStatusUI;
    [SerializeField]
    private GameObject _aiStatusUI;

    [SerializeField] GameObject turnstopUI;
    private TextMeshProUGUI _turnstopText;
    public bool isEmphasis = false;

    private Text _goalText;

    [SerializeField] private GameObject _broadTextObj;
    private TextMeshProUGUI _broadText;

    private void Awake()
    {
        _goalText = goalUI.transform.GetComponentInChildren<Text>();

        _broadText = _broadTextObj.GetComponent<TextMeshProUGUI>();

        _turnstopText = turnstopUI.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void UpdateTurnText()
    {
        turnText.text = string.Format("앞으로 {0}턴", FightManager.Instance.turn);
    }

    public void UpdateGoalUI()
    {
        _goalText.transform.DORewind();
        Sequence seq = DOTween.Sequence();
        seq.Append(goalUI.transform.GetChild(3).transform.DOScaleX((float)FightManager.Instance.aiList.Count / 3, 1.5f));
        seq.Append(_goalText.transform.DOShakeScale(0.4f, 0.7f, 5));
        seq.AppendCallback(() =>
        {
            _goalText.text = FightManager.Instance.aiList.Count.ToString();
        });
    }

    public void ShowPlayerStatusUI(Player p)
    {
        _playerStatusUI.SetActive(true);

        _playerStatusUI.transform.Find("Name").GetComponent<Text>().text = p.playerName;
        _playerStatusUI.transform.Find("Info").GetComponent<TextMeshProUGUI>().text = p.info;

        GameObject durabilityBar = _playerStatusUI.transform.Find("DurabilityPoint").gameObject;
        GameObject kineticPoint = _playerStatusUI.transform.Find("KineticPoint").gameObject;

        durabilityBar.transform.Find("valueText").GetComponent<Text>().text = p.DurabilityPoint.ToString();
        kineticPoint.transform.Find("valueText").GetComponent<Text>().text = p.KineticPoint.ToString();

        durabilityBar.transform.DOScaleX((float)p.DurabilityPoint / p.MaxDurabilityPoint, 0);
        kineticPoint.transform.DOScaleX((float)p.KineticPoint / p.MaxKineticPoint, 0);
    }

    public void ShowAIStatusUI(AI ai)
    {
        
        _aiStatusUI.SetActive(true);

        _aiStatusUI.transform.Find("Name").GetComponent<Text>().text = ai.aiName;
        _aiStatusUI.transform.Find("Info").GetComponent<TextMeshProUGUI>().text = ai.info;

        GameObject influenceBar = _aiStatusUI.transform.Find("InfluencePoint").gameObject;
        influenceBar.transform.Find("valueText").GetComponent<Text>().text = ai.InfluencePoint.ToString();
        influenceBar.transform.DOScaleX((float)ai.InfluencePoint / ai.MaxInfluencePoint, 0);
    }

    public void HidePlayerStatusUI()
    {
        _playerStatusUI.SetActive(false);
    }

    public void HideAIStatusUI()
    {
        _aiStatusUI.SetActive(false);
    }

    public void ViewText(string text, Action action = null)
    {
        _broadTextObj.SetActive(true);

        _broadText.text = text;
        
        Sequence seq = DOTween.Sequence();
        seq.Append(_broadText.DOFade(1f, 1f));
        seq.AppendInterval(0.5f);
        seq.Append(_broadText.DOFade(0f, 1f));
        seq.AppendCallback(()=>
        {
            action?.Invoke();
        });
        seq.AppendCallback(() =>
        {
            _broadTextObj.SetActive(false);
        }); 
    }

    public void ShowInfoUI(bool isActive)
    {
        infoUI.SetActive(isActive);
    }

    public void TurnstopEmphasis()
    {
        isEmphasis = true;

        Sequence seq = DOTween.Sequence();
        seq.Append(_turnstopText.transform.DOScale(new Vector2(1.2f, 1.2f), 0.3f));
        seq.Append(_turnstopText.transform.DOScale(new Vector2(1f, 1f), 0.3f));
        seq.AppendInterval(0.5f);
        seq.SetLoops(-1);
    }

    public void TurnstopEmphasisStop()
    {
        DOTween.KillAll();
    }
}
