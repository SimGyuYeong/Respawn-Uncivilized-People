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
        RobinSkeletonAnimation = gameObject.transform.GetChild(0).gameObject.GetComponent<SkeletonAnimation>();
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
    //앞에 애니메이션이 끝나고 실행     무슨 애니메이션인지          루프를 할 건지       몇 초후에 시작할 건지
    public void RobinAnimationAdd(int robinAnimationIndex, bool animationLoop, float animationDelay)
    {
        switch(robinAnimationIndex)
        {
            case 0: // 로빈 IDLE 애니메이션
                RobinSkeletonAnimation.AnimationState.AddAnimation(0, "Idle", animationLoop, animationDelay);
                break;
            case 1: // 로빈 blink 애니메이션
                RobinSkeletonAnimation.AnimationState.AddAnimation(0, "blink", animationLoop, animationDelay);
                break;
            //case 2: // 로빈 IDLE+BLICK 애니메이션
            //    RobinSkeletonAnimation.AnimationState.Data.SetMix
        }
    }


    //앞에 애니메이션 무시하고 실행     무슨 애니메이션인지          루프를 할 건지       몇 초후에 시작할 건지

    public void RobinAnimationSet(int RobinAnimationIndex, bool animationLoop, float animationDelay)
    {
        switch (RobinAnimationIndex)
        {
            case 0:

                break;
        }
    }
}
