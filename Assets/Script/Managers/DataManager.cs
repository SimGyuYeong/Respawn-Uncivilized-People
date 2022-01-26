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
            for (int j = 1; j < 4; j++)
            {
                GameObject obj = menu[i].transform.GetChild(j).gameObject;
                if (j == 3)
                {
                    Image image = obj.GetComponent<Image>();
                    image.sprite = nullImage;
                    if (data[i].id != 0) {
                        image.sprite = GameManager.Instance.TEXT.background[data[i].imageID].GetComponent<Image>().sprite;
                    }
                }
                else
                {
                    Text text = obj.GetComponent<Text>();
                    if (j == 1) text.text = data[i].playerName;
                    else text.text = data[i].date;
                }
                
            }
        }
    }

    public void SaveMenuPanelOpen(int num)
    {
        if (num == 0) SaveOrLoad(0);
        else if (num == 3) SaveOrLoad(3);
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
            data[slot].playerName = GameManager.Instance.PlayerName;
            data[slot].date = DateTime.Now.ToString("yyyy�� MM�� dd��\n HH:mm:ss");
            data[slot].id = GameManager.Instance.TEXT.chatID;
            data[slot].typdingID = GameManager.Instance.TEXT.typingID;
            data[slot].imageID = GameManager.Instance.TEXT.backID;
            SaveToJson();
            SaveMenuPanelUpdate();
        }
        else
        {
            if (slot == 3) slot = 0;
            StartCoroutine(GameManager.Instance.FadeIn());
            GameManager.Instance.TEXT.chatID = data[slot].id;
            GameManager.Instance.TEXT.typingID = data[slot].typdingID;
            GameManager.Instance.PlayerName = data[slot].playerName;
            GameManager.Instance.TitlePanel.SetActive(false);
            GameManager.Instance.Buttons.SetActive(false);
            SaveMenuPanelClose();
            StartCoroutine(GameManager.Instance.TEXT.LoadTextData());
            GameManager.Instance.TEXT.background[GameManager.Instance.TEXT.backID].SetActive(false);
            StartCoroutine(GameManager.Instance.TEXT.Typing());
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