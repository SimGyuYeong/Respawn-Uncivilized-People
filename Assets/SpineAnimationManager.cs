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
    //�տ� �ִϸ��̼��� ������ ����     ���� �ִϸ��̼�����          ������ �� ����       �� ���Ŀ� ������ ����
    public void RobinAnimationAdd(int robinAnimationIndex, bool animationLoop, float animationDelay)
    {
        switch(robinAnimationIndex)
        {
            case 0: // �κ� IDLE �ִϸ��̼�
                RobinSkeletonAnimation.AnimationState.AddAnimation(0, "Idle", animationLoop, animationDelay);
                break;
            case 1: // �κ� blink �ִϸ��̼�
                RobinSkeletonAnimation.AnimationState.AddAnimation(0, "blink", animationLoop, animationDelay);
                break;
            //case 2: // �κ� IDLE+BLICK �ִϸ��̼�
            //    RobinSkeletonAnimation.AnimationState.Data.SetMix
        }
    }


    //�տ� �ִϸ��̼� �����ϰ� ����     ���� �ִϸ��̼�����          ������ �� ����       �� ���Ŀ� ������ ����

    public void RobinAnimationSet(int RobinAnimationIndex, bool animationLoop, float animationDelay)
    {
        switch (RobinAnimationIndex)
        {
            case 0:

                break;
        }
    }
}
