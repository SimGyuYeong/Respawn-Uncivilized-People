using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FightManager : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Content;

    [SerializeField] Text turnText;

    [SerializeField] GameObject TileUI;
    [SerializeField] GameObject GoalUI;

    [SerializeField] GameObject TilePrefab;

    private int energy = 100;
    private bool isClickPlayer = false;
    private int enemyCount = 3;
    [SerializeField] int turn = 10;

    public void Start()
    {
        UpdateUI();
        StartCoroutine(spawnTile());
    }


    IEnumerator spawnTile()
    {
        int count = 1;
        float y = 3.9f;
        for (int i = 0; i < 8; i++)
        {
            float x = -7.7f;
            for (int j = 0; j < 8; j++)
            {
                var spawnedTile = Instantiate(TilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.transform.parent = Content.transform;
                spawnedTile.name = $"Tile {count}";
                count++;
                if(i == 0) yield return new WaitForSeconds(0.08f);
                x += 1.1f;
            }
            yield return new WaitForSeconds(0.08f);
            y -= 1.1f;
        }
    }

    public void OnClickTile(GameObject obj)
    {
        if (isClickPlayer)
        {
            string position = (Player.transform.position - obj.transform.position).ToString();
            if (Player.transform.parent == obj.transform) return;
            else if (energy < 5) return;
            else if (position == new Vector3(1.1f, 0).ToString() || position == new Vector3(-1.1f, 0).ToString() || position == new Vector3(0, 1.1f).ToString() || position == new Vector3(0, -1.1f).ToString())
            {
                if (obj.transform.childCount > 0)
                {
                    int damage = System.Convert.ToInt32(obj.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text);
                    energy -= damage;
                    enemyCount--;
                }
                else energy -= 5;

                if (obj.transform.childCount > 0)
                {
                    Player.transform.DOMove(obj.transform.position, 2.0f)
                        .OnComplete(() => StartCoroutine(SoftGoalValueChange()))
                        .OnComplete(() => Player.transform.SetParent(obj.transform));
                    Destroy(obj.transform.GetChild(0).gameObject);
                }
                else
                    Player.transform.DOMove(obj.transform.position, 2.0f)
                        .OnComplete(() => Player.transform.SetParent(obj.transform));

                OnClickPlayer();
                turn--;
                UpdateUI();
            }
        }
    }

    private void UpdateUI()
    {
        turnText.text = string.Format("앞으로 {0}턴", turn);
        StartCoroutine(SoftTileValueChange());
    }

    private IEnumerator SoftTileValueChange()
    {
        int value = (int)TileUI.transform.GetChild(2).GetComponent<Slider>().value;
        Text energyText = TileUI.transform.GetChild(1).GetComponent<Text>();
        while (value != energy)
        {
            TileUI.transform.GetChild(2).GetComponent<Slider>().value--;
            value = (int)TileUI.transform.GetChild(2).GetComponent<Slider>().value;
            energyText.text = string.Format("{0} / 100", value);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator SoftGoalValueChange()
    {
        float value = GoalUI.transform.GetChild(2).GetComponent<Slider>().value;
        Text countText = GoalUI.transform.GetChild(1).GetComponent<Text>();
        while (value != enemyCount)
        {
            GoalUI.transform.GetChild(2).GetComponent<Slider>().value--;
            value = GoalUI.transform.GetChild(2).GetComponent<Slider>().value;
            countText.text = string.Format("{0} / 3", value);
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void OnClickPlayer()
    {
        isClickPlayer = !isClickPlayer;
        int childNum = 29;
        if (isClickPlayer)
        {
            Content.transform.GetChild(childNum - 1).gameObject.GetComponent<Image>().color = Color.yellow;
            Content.transform.GetChild(childNum + 1).gameObject.GetComponent<Image>().color = Color.yellow;
            Content.transform.GetChild(childNum - 8).gameObject.GetComponent<Image>().color = Color.yellow;
            Content.transform.GetChild(childNum + 8).gameObject.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            Content.transform.GetChild(childNum - 1).gameObject.GetComponent<Image>().color = Color.white;
            Content.transform.GetChild(childNum + 1).gameObject.GetComponent<Image>().color = Color.white;
            Content.transform.GetChild(childNum - 8).gameObject.GetComponent<Image>().color = Color.white;
            Content.transform.GetChild(childNum + 8).gameObject.GetComponent<Image>().color = Color.white;
        }
    }
}
