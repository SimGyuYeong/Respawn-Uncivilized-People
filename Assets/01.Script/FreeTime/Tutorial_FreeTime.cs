using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_FreeTime : MonoBehaviour
{
    private FreeTimeText _freeTimeText = null;

    private void Awake()
    {
        _freeTimeText = transform.parent.GetComponent<FreeTimeText>();
    }

    public void StartTuto()
    {
        StartCoroutine(Tutorial_Co());
    }

    IEnumerator Tutorial_Co()
    {
        yield return new WaitForSeconds(0.7f);
        _freeTimeText.SetText(5);
        for (int i = 0; i < 4; i++)
        {
            ButtonManager.Instance.storyButton[i].interactable = false;
        }
    }
}