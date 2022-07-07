using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSave : MonoBehaviour
{
    public static int STAGE = 0;

    public static void StageStart()
    {
        STAGE++;
        FightManager.Instance.StageStart(STAGE);
    }
}
