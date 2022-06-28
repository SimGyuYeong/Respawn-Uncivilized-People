using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightSoundManager : MonoBehaviour
{
    [SerializeField] // BGM ��� ���� ����� �ҽ�
    private AudioSource BGM;

    [SerializeField] // Effect ��� ���� ����� �ҽ�
    private AudioSource Effect1;

    [SerializeField] // Effect ��� ���� ���� ����� �ҽ�
    private AudioSource Effect2;

    [SerializeField] // �������, ȿ���� ��� ����� �ҽ� �迭
    private AudioClip[] Sounds;

    enum SoundsIndexNumber 
    {
        BGM,
        GameOver,
        PlayerMove,
        EnemyMove,
        ButtonClick,
        PlayerClick,
        TurnEndButton,
        AttackEffect,
        Effect2
    };

    private void Awake()
    {

    }

    void Update()
    {
        
    }

    public void BGMSet()
    {
        BGM.clip = Sounds[0];
    }
}
