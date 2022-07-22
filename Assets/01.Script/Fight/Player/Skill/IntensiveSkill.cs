using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntensiveSkill : Skill
{
    protected override void ShowDistance()
    {
        base.ShowDistance();
        foreach (var ai in _attackAI)
        {
            if (ai.GetComponentInChildren<AI>().IsRestructuring() == true)
            {
                ai.transform.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
}
