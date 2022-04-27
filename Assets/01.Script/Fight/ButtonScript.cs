using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public FightManager.Action actionName;

    public void ButtonClick()
    {
        FightManager.Instance.PlayerAction(actionName);
    }
}
