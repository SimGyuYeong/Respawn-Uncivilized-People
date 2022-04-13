using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    [SerializeField]
    private Sprite[] storeImage;

    [SerializeField]
    private Sprite[] itemImage;

    [SerializeField]
    private Image storePanel;

    [SerializeField]
    private Image itemPanel;

    public void ChangeStorePanel(int selectButton)
    {
        storePanel.sprite = storeImage[selectButton];
        itemPanel.sprite = itemImage[selectButton];
    }
}
