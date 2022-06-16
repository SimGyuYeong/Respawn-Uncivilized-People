using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using System;
using DG.Tweening;

class saveData
{
    public int id;
    public string date;
    public int typdingID;
    public int imageID;

    public saveData(int id, string date, int typing, int image)
    {
        this.id = id;
        this.date = date;
        typdingID = typing;
        imageID = image;
    }
}


public class DataManager : MonoBehaviour
{
    Dictionary<int, saveData> data = new Dictionary<int, saveData>();

    [SerializeField] public GameObject savemenuPanel;
    [SerializeField] private Sprite nullImage;
    [SerializeField] List<GameObject> menu = new List<GameObject>();

    

    private int SLCheck = 1;

    private void Start()
    {
        LoadFromJson();
        SaveMenuPanelUpdate();
    }

    private void SaveMenuPanelUpdate()
    {
        for (int i = 0; i < 8; i++)
        {
            if (data[i].date != " ")
            {
                Transform trm = menu[i].transform;

                trm.Find("Date").GetComponent<Text>().text = data[i].date;
                trm.Find("Image").GetComponent<Image>().sprite = TextManager.Instance.TextSO.backgroundList[data[i].imageID].GetComponent<SpriteRenderer>().sprite;
            }
            
        }
    }

    public void SaveMenuPanelOpen(int num)
    {
        if (num == 0)
        {
            SLCheck = 1;
            SaveOrLoad(0);
        }
        else if (num == 3)
        {
            SLCheck = 2;
            SaveOrLoad(0);
        }
        else
        {
            TextUIManager.instance.Loading(() =>
            {
                savemenuPanel.SetActive(true);
                SLCheck = num;
                if (!TextManager.Instance.isTyping) TextManager.Instance.endAnimationObj.SetActive(false);
            });
        }
    }

    public void SaveMenuPanelClose()
    {
        TextUIManager.instance.Loading(() => {
            savemenuPanel.SetActive(false);
            if (!TextManager.Instance.isTyping) TextManager.Instance.endAnimationObj.SetActive(true);
        });
    }

    public void SaveOrLoad(int slot)
    {
        
        if (SLCheck < 2)
        {
            data[slot].date = DateTime.Now.ToString("yyyy³â MM¿ù ddÀÏ\n HH:mm:ss");
            data[slot].id = GameManager.Instance.TEXT.chatID;
            data[slot].typdingID = GameManager.Instance.TEXT.lineNumber;
            data[slot].imageID = GameManager.Instance.TEXT.backgroundID;
            SaveToJson();
            SaveMenuPanelUpdate();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(GameManager.Instance.FadeIn());
            GameManager.Instance.TEXT.chatID = data[slot].id;
            GameManager.Instance.TEXT.lineNumber = data[slot].typdingID;
            GameManager.Instance.TitlePanel.SetActive(false);
            GameManager.Instance.Buttons.SetActive(false);
            SaveMenuPanelClose();
            TextManager.Instance.TextSO.backgroundList[TextManager.Instance.backgroundID].SetActive(false);
            TextManager.Instance.TextTyping?.Invoke();
        }
    }

    public void SaveToJson()
    {
        string saveData = JsonConvert.SerializeObject(data);
        File.WriteAllText(Path.Combine(Application.dataPath, "saveData.json"), saveData);
    }

    public void LoadFromJson()
    {
        if (File.Exists(Path.Combine(Application.dataPath, "saveData.json"))){
            string saveData = File.ReadAllText(Path.Combine(Application.dataPath, "saveData.json"));
            data = JsonConvert.DeserializeObject<Dictionary<int, saveData>>(saveData);
        }
        else
        {
            for(int i = 0; i < 8; i++)
            {
                data[i] = new saveData(0, " ", 0, 0);
            }
            SaveToJson();
        }   
        
    }
}