using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
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
    public Image _turnstopObj;
    public bool isEmphasis = false;

    private Text _goalText;

    #region 전체 메세지
    [SerializeField] private GameObject _broadcastObj;
    private Image _panel;
    private Image _background; 
    private TextMeshProUGUI _broadText;
    #endregion

    #region 스킬버튼
    [SerializeField] private GameObject _skillUI;
    [SerializeField] private GameObject _skillInfoUI;

    public Skill selectSkill;
    public Transform skillInfoTrm;

    private List<Image> _skillButton = new List<Image>();
    #endregion

    private void Awake()
    {
        _goalText = goalUI.transform.GetComponentInChildren<Text>();

        _background = _broadcastObj.transform.Find("background").GetComponent<Image>();
        _panel = _broadcastObj.transform.Find("panel").GetComponent<Image>();
        _broadText = _broadcastObj.transform.Find("text").GetComponent<TextMeshProUGUI>();

        _skillButton.Add(_skillUI.transform.Find("IronFist").GetComponent<Image>());
        _skillButton.Add(_skillUI.transform.Find("IntensiveAttack").GetComponent<Image>());
        _skillButton.Add(_skillUI.transform.Find("KnockDown").GetComponent<Image>());
        _skillButton.Add(_skillUI.transform.Find("SuppressionDrone").GetComponent<Image>());
        skillInfoTrm = _skillUI.transform.Find("Info");
    }

    public void UpdateTurnText()
    {
        turnText.text = string.Format("앞으로 {0}턴", FightManager.Instance.turn);
    }

    public void UpdateGoalUI()
    {
        _goalText.transform.DORewind();
        Sequence seq = DOTween.Sequence();
        seq.Append(goalUI.transform.GetChild(3).transform.DOScaleX((float)FightManager.Instance.aiList.Count / FightManager.Instance.aiDataList.Count, 1.5f));
        seq.Append(_goalText.transform.DOShakeScale(0.4f, 0.7f, 5));
        seq.AppendCallback(() =>
        {
            _goalText.text = FightManager.Instance.aiList.Count.ToString();
            _goalText.transform.DORewind();
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

        durabilityBar.transform.Find("sprite").DOScaleX((float)p.DurabilityPoint / p.MaxDurabilityPoint, 0);
        kineticPoint.transform.Find("sprite").DOScaleX((float)p.KineticPoint / p.MaxKineticPoint, 0);
    }

    public void ShowAIStatusUI(AI ai)
    {
        
        _aiStatusUI.SetActive(true);

        _aiStatusUI.transform.Find("Name").GetComponent<Text>().text = ai.Name;
        _aiStatusUI.transform.Find("Info").GetComponent<TextMeshProUGUI>().text = ai.Info;

        GameObject influenceBar = _aiStatusUI.transform.Find("InfluencePoint").gameObject;
        influenceBar.transform.Find("valueText").GetComponent<Text>().text = ai.InfluencePoint.ToString();
        influenceBar.transform.Find("sprite").DOScaleX((float)ai.InfluencePoint / ai.MaxInfluencePoint, 0);
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
        _broadcastObj.SetActive(true);
        _background.DOFade(0.4f, 0.2f);

        _broadText.text = text;

        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => StartCoroutine(TextShow(action)));
        seq.Append(_panel.gameObject.transform.DOScale(Vector3.one * 1.5f, 0));
        seq.Append(_panel.DOFade(1, 0.2f));
        seq.Join(_panel.gameObject.transform.DOScale(Vector3.one, 0.2f));
    }

    IEnumerator TextShow(Action action)
    {
        yield return new WaitForSeconds(0.1f);
        Sequence seq = DOTween.Sequence();
        seq.Append(_broadText.gameObject.transform.DOScale(Vector3.one * 1.3f, 0));
        seq.Append(_broadText.DOFade(1, 0.2f));
        seq.Join(_broadText.gameObject.transform.DOScale(Vector3.one, 0.2f));
        seq.AppendInterval(1f);

        //사라지는거
        seq.AppendCallback(() => StartCoroutine(BackHide(action)));
        seq.Append(_broadText.gameObject.transform.DOScale(Vector3.one * 1.3f, 0.2f));
        seq.Join(_broadText.DOFade(0, 0.2f));
        
    }

    IEnumerator BackHide(Action action)
    {
        yield return new WaitForSeconds(0.1f);
        Sequence seq = DOTween.Sequence();
        seq.Append(_panel.gameObject.transform.DOScale(Vector3.one * 1.3f, 0.2f));
        seq.Join(_panel.DOFade(0, 0.2f));
        seq.Append(_background.DOFade(0, 0.2f));
        seq.AppendCallback(() =>
        {
            action?.Invoke();
            _broadcastObj.SetActive(false);
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
        seq.Append(_turnstopObj.transform.DOScale(Vector3.one * 1.5f, 1f));
        seq.Join(_turnstopObj.DOFade(0, 1f));
        seq.AppendInterval(0.5f);
        seq.SetLoops(-1);
    }

    public void TurnstopEmphasisStop()
    {
        DOTween.KillAll();
        _turnstopObj.transform.localScale = Vector3.one;
    }

    public void ShowSkillUI(bool value)
    {
        _skillUI.SetActive(value);

        if (value == true)
        {
            foreach (var button in _skillButton)
            {
                button.color = button.GetComponent<Skill>().SkillWhether() ? Color.white : Color.gray;
            }
        }
    }

    public void SkillInfoUIShow(bool value, Vector3 pos)
    {
        _skillInfoUI.transform.localPosition = pos;
        if(value == true)
            _skillInfoUI.GetComponent<Image>().DOFade(1, .2f);
        else
            _skillInfoUI.GetComponent<Image>().DOFade(0, .2f);
    }
}
