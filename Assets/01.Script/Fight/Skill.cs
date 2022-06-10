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
        SuppressionDrone,
        None
    }

    public enum SkillCost
    {
        IronFist = 15,
        IntensiveAttack = 20,
        KnockDown = 50,
        SuppressionDrone = 25
    }

    public void Skilling(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.IronFist:
                if (FightManager.Instance.player.KineticPoint >= (int)SkillCost.IronFist)
                    FightManager.Instance.UI.SelectSkillButton((int)SkillType.IronFist);
                break;
            case SkillType.IntensiveAttack:
                if (FightManager.Instance.player.KineticPoint >= (int)SkillCost.IntensiveAttack)
                    FightManager.Instance.UI.SelectSkillButton((int)SkillType.IntensiveAttack);
                break;
            case SkillType.KnockDown:
                if (FightManager.Instance.player.KineticPoint >= (int)SkillCost.KnockDown)
                    FightManager.Instance.UI.SelectSkillButton((int)SkillType.KnockDown);
                break;
            case SkillType.SuppressionDrone:
                if (FightManager.Instance.player.KineticPoint >= (int)SkillCost.SuppressionDrone)
                    FightManager.Instance.UI.SelectSkillButton((int)SkillType.SuppressionDrone);
                break;
            default:
                break;
        }
    }

    public void ShowSkillDIstance(SkillType skill)
    {
        if(FightManager.Instance.UI.selectSkillNum == 5)
        {
            switch (skill)
            {
                case SkillType.IronFist:
                    if (FightManager.Instance.player.KineticPoint >= (int)SkillCost.IronFist)
                        FightManager.Instance.ShowDistance(1, true);
                    break;
                case SkillType.IntensiveAttack:
                    if (FightManager.Instance.player.KineticPoint >= (int)SkillCost.IntensiveAttack)
                        FightManager.Instance.ShowDistance(3, true);
                    break;
                case SkillType.KnockDown:
                    if (FightManager.Instance.player.KineticPoint >= (int)SkillCost.KnockDown)
                        FightManager.Instance.ShowDistance(1, true);
                    break;
                case SkillType.SuppressionDrone:
                    if (FightManager.Instance.player.KineticPoint >= (int)SkillCost.SuppressionDrone)
                        FightManager.Instance.ShowDistance(2, true);
                    break;
                default:
                    break;
            }
        }
        
    }

    public void SelectButtonExit()
    {
        if(FightManager.Instance.UI.selectSkillNum == 5)
        {
            FightManager.Instance.HideDistance();
        }
    }
}
