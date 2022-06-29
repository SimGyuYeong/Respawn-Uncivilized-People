using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronFistSkill : Skill
{
    protected override void SelectSkillButton()
    {
        FightManager.Instance.player.KineticPoint -= _cost;

        _damagedAIList.Clear();
        foreach(var tile in _attackAI)
        {
            _damagedAIList.Add(tile.GetComponentInChildren<AI>());
        }

        AIDamage();
    }
}
