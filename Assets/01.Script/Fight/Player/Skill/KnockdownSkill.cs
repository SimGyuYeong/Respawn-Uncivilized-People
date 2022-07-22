using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockdownSkill : Skill
{
    protected override void AIDamage()
    {
        base.AIDamage();
        foreach(var ai in _damagedAIList)
        {
            if(ai.IsRestructuring())
            {
                ai.Death();
            }
        }
    }

    
}
