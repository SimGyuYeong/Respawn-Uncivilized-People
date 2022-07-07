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
            if(ai.GetComponentInChildren<AI>().IsRestructuring() == false)
            {
                _count--;
            }
        }

        return _count;
    }

    protected override void ShowDistance()
    {
        CheckDistance();
        _distanceTiles.ForEach(x => x.GetComponent<SpriteRenderer>().color = Color.red);

        foreach (var ai in _attackAI)
        {
            if (ai.GetComponentInChildren<AI>().IsRestructuring() == false)
            {
                ai.transform.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
}
