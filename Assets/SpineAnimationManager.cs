using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

public class SpineAnimationManager : MonoBehaviour
{
    public SkeletonAnimation RobinSkeletonAnimation;
    private SkeletonAnimation SaeSkeletonAnimation;
    private SkeletonAnimation BiyeonSkeletonAnimation;
    //private AnimationStateDate 

    private void Awake()
    {
        RobinSkeletonAnimation = GameObject.Find("Robin").GetComponentInChildren<SkeletonAnimation>();
        //SaeSkeletonAnimation = GameObject.Find("Sae").GetComponent<SkeletonAnimation>();
        //BiyeonSkeletonAnimation = GameObject.Find("Biyeon").GetComponent<SkeletonAnimation>();
    }

    private void Start()
    {
        
        RobinSkeletonAnimation.AnimationState.AddAnimation(0, "Idle+Blink", true , 0f);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //RobinSkeletonAnimation.AnimationState.Data.SetMix("blink", "Idle", 0.5f);
            RobinSkeletonAnimation.AnimationState.AddAnimation(0, "Idle", true, 0);
            RobinSkeletonAnimation.AnimationState.AddAnimation(1, "blink", true, 0);
        }
    }
}
