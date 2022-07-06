using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSave : MonoBehaviour
{
    public static int STAGE = 0;
    public static StageSave instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void StageStart()
    {
        STAGE++;
        FightManager.Instance.StageStart(STAGE);
    }
}
