using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public Skill.SkillType skillType = Skill.SkillType.IronFist;

    public UnityEvent<Skill.SkillType> OnClickEvent;
    public UnityEvent<Skill.SkillType> OnEnterEvent;
    public UnityEvent<Skill.SkillType> OnExitEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClickEvent?.Invoke(skillType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnterEvent?.Invoke(skillType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExitEvent?.Invoke(skillType);
    }
}
