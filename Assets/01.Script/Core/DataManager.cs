using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using System;


class saveData
{
    public string playerName;
    public int id;
    public string date;
    public int typdingID;
    public int imageID;

    public saveData(string name, int id, string date, int typing, int image)
    {
        playerName = name;
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
            for (int j = 1; j < 3; j++)
            {
                GameObject obj = menu[i].transform.GetChild(j).gameObject;
                if (j == 2)
                {
                    SpriteRenderer image = obj.GetComponent<SpriteRenderer>();
                    image.sprite = nullImage;
                    if (data[i].id != 0) {
                        image.sprite = TextManager.Instance.TextSO.backgroundList[data[i].imageID].GetComponent<SpriteRenderer>().sprite;
                    }
                }
                else
                {
                    Text text = obj.GetComponent<Text>();
                    if (j == 0) text.text = data[i].playerName;
                    else text.text = data[i].date;
                }
                
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
            Time.timeScale = 0f;
            savemenuPanel.SetActive(true);
            SLCheck = num;
        }
    }

    public void SaveMenuPanelClose()
    {
        Time.timeScale = 1f;
        savemenuPanel.SetActive(false);
    }

    public void SaveOrLoad(int slot)
    {
        
        if (SLCheck < 2)
        {
            data[slot].playerName = GameManager.Instance.playerName;
            data[slot].date = DateTime.Now.ToString("yyyy³â MM¿ù ddÀÏ\n HH:mm:ss");
            data[slot].id = GameManager.Instance.TEXT.chatID;
            data[slot].typdingID = GameManager.Instance.TEXT.typingID;
            data[slot].imageID = GameManager.Instance.TEXT.backgroundID;
            SaveToJson();
            SaveMenuPanelUpdate();
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(GameManager.Instance.FadeIn());
            GameManager.Instance.TEXT.chatID = data[slot].id;
            GameManager.Instance.TEXT.typingID = data[slot].typdingID;
            GameManager.Instance.playerName = data[slot].playerName;
            GameManager.Instance.TitlePanel.SetActive(false);
            GameManager.Instance.Buttons.SetActive(false);
            SaveMenuPanelClose();
            StartCoroutine(GameManager.Instance.TEXT.LoadTextData());
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
                data[i] = new saveData("???", 0, " ", 0, 0);
            }
            SaveToJson();
        }   
        
    }
}