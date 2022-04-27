using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItemPanel : MonoBehaviour
{
    private List<ItemPanelSetting> itemPanelsList = new List<ItemPanelSetting>();
    public GameObject content;
    public ItemPanelSetting itemPanel;

    public void SpawnPanel()
    {
        StartCoroutine(PanelOn());
    }


    IEnumerator PanelOn()
    {
        for (int i = 0; i < 18; i++)
        {
            var spawnItemPanel = Instantiate(itemPanel, Vector3.zero, Quaternion.identity);
            spawnItemPanel.transform.parent = content.transform;
            yield return new WaitForSeconds(0.2f);

            itemPanelsList.Add(spawnItemPanel);
        }
    }
}
