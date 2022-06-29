using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuppressionSkill : Skill
{
    protected override int CountingAI()
    {
        CheckDistance();
        int _count = _attackAI.Count;
        foreach(var ai in _attackAI)
        { 
            if(ai.GetComponentInChildren<AI>().isRestructuring == false)
            {
                _count--;
            }
        }

        return _count;
    }

    protected override void ShowDistance()
    {
        base.ShowDistance();
        foreach(var ai in _attackAI)
        {
            if (ai.GetComponentInChildren<AI>().isRestructuring == false)
            {
                ai.transform.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
}
