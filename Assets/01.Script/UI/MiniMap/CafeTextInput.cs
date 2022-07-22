using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cafe TextData", menuName = "Scriptable Object/Cafe TextData", order = 1)]
public class CafeTextInput : ScriptableObject
{
    [SerializeField]
    public string[] CharacterName;  //���� ��縦 ġ�� ���ΰ�..�̸�
    [SerializeField]
    public string[] CafeText;  //��� ���� ��縦 ġ�� ���ΰ�... ���...ī��
    [SerializeField]
    public string[] DepartmentText;  //��� ���� ��縦 ġ�� ���ΰ�... ���...��ȭ��
    [SerializeField]
    public string[] ParkText;  //��� ���� ��縦 ġ�� ���ΰ�... ���...����
    [SerializeField]
    public string[] RestaurantText;  //��� ���� ��縦 ġ�� ���ΰ�... ���...�������
}