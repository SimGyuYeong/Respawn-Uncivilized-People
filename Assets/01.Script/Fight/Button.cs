using UnityEngine;

public class Button : MonoBehaviour
{
    public FightManager.Action actionName;

    public void ButtonClick()
    {
        FightManager.Instance.PlayerAction(actionName);
    }
}
