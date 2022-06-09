using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{

    public enum SkillType
    {
        IronFist,
        IntensiveAttack,
        KnockDown,
        SuppressionDrone
    }

    public void Skilling(SkillType skill)
    {
        
    }

    public void ShowSkillDIstance(SkillType skill)
    {
        switch(skill)
        {
            case SkillType.IronFist:
                FightManager.Instance.ShowDistance(1);
                break;
            case SkillType.IntensiveAttack:
                FightManager.Instance.ShowDistance(3);
                break;
            case SkillType.KnockDown:
                FightManager.Instance.ShowDistance(1);
                break;
            case SkillType.SuppressionDrone:
                FightManager.Instance.ShowDistance(2);
                break;
            default:
                break;
        }
    }
}
