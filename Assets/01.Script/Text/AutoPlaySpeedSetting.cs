using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlaySpeedSetting : MonoBehaviour
{
    public void SetAutoplaySpeed(float speed)
    {
        GameManager.Instance.TEXT.autoSpeed = speed;
        PlayerPrefs.SetFloat("auto", speed);
    }
}
