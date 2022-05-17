using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class RobinAnimation : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset[] AnimClip;

    //애니메이션 Enum
    public enum AnimState { };

    //현재 애니메이션 처리
    private AnimState _AnimState;
    // 현재 실행 중인 애니메이션
    private string CurrentAnimation;

    private Rigidbody2D rig;
    private float xx;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        xx = Input.GetAxisRaw("Horizontal");
    }
}
