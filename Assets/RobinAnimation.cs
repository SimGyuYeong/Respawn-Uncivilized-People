using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class RobinAnimation : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset[] AnimClip;

    //�ִϸ��̼� Enum
    public enum AnimState { };

    //���� �ִϸ��̼� ó��
    private AnimState _AnimState;
    // ���� ���� ���� �ִϸ��̼�
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
