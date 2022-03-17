using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class BeforeFM : MonoBehaviour
{
    //싱글톤
    public static BeforeFM Instance;

    //플레이어
    public GameObject Player;
    public bool _playerClick = false;
    public int PlayerSpawnTN = 10;
    public int MoveEnergy = 2;

    //타일 관련
    [SerializeField] GameObject Content;
    [SerializeField] Tile TilePrefab;
    public bool[] tileActiveList;
    public List<GameObject> tileList = new List<GameObject>();
    public int csTileNum = 1;

    //적AI
    [SerializeField] GameObject Enemy;
    public int[] EnemySpawnTN;

    [SerializeField] Text turnText;

    [SerializeField] GameObject TileUI;
    [SerializeField] GameObject GoalUI;

    private int energy = 100;
    private int enemyCount = 3;
    [SerializeField] int turn = 10;

    public void Awake()
    {
        Instance = this;
    }

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
                spawnedTile.tileNum = count;
                tileList.Add(spawnedTile.gameObject);

                if (count == PlayerSpawnTN)
                {
                    Player = Instantiate(Player, spawnedTile.transform);
                    Player.transform.position = spawnedTile.transform.position;
                }

                foreach(int value in EnemySpawnTN)
                {
                    if(value == count)
                    {
                        var sEnemy = Instantiate(Enemy, spawnedTile.transform);
                        sEnemy.transform.position = spawnedTile.transform.position;
                    }
                }

                if (tileActiveList[count - 1] == true)
                {
                    spawnedTile.notActive();
                    spawnedTile.GetComponent<SpriteRenderer>().color = Color.gray;
                }
                
                count++;
                if(i == 0) yield return new WaitForSeconds(0.08f);
                x += 1.1f;
            }
            yield return new WaitForSeconds(0.08f);
            y -= 1.1f;
        }
    }

    IEnumerator DestroyTile()
    {
        foreach(var tile in tileList)
        {
            Destroy(tile);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void PlayerMove(int tileNum)
    {
        GameObject obj = tileList[tileNum - 1].gameObject;
        
        if (energy < MoveEnergy)
        {
            energy--;
            turn--;
        }
        else if (obj.GetComponent<SpriteRenderer>().color == Color.yellow)
        {
            energy -= MoveEnergy;
            Player.transform.DOMove(obj.transform.position, 1.0f)
                    .OnComplete(() => Player.transform.SetParent(obj.transform));
        }
        else if (obj.GetComponent<SpriteRenderer>().color == Color.red)
        {
            TextMeshProUGUI textMesh = obj.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
            int damage = System.Convert.ToInt32(textMesh.text);
            energy -= damage / 2;
            enemyCount--;
            AttackEnemy(obj.transform.GetChild(1).gameObject);
        }

        OnClickPlayer();
        csTileNum = tileNum;
        turn--;
        if (turn == 0)
        {
            StartCoroutine(DestroyTile());
            Debug.Log("Stage Over");
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        turnText.text = string.Format("앞으로 {0}턴", turn);
        StartCoroutine(SoftTileValueChange());
        StartCoroutine(SoftGoalValueChange());
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
        _playerClick = !_playerClick;

        if (_playerClick) //플레이어 클릭시, 이동가능거리 표시
        {
            if (csTileNum % 8 != 0) tileList[csTileNum].GetComponent<Tile>().enter(true); //우측표시
            if ((csTileNum - 1) % 8 != 0) tileList[csTileNum - 2].GetComponent<Tile>().enter(true); //좌측표시
            if (csTileNum <= 56) tileList[csTileNum + 7].GetComponent<Tile>().enter(true); //하단표시
            if (csTileNum > 8) tileList[csTileNum - 9].GetComponent<Tile>().enter(true); //상단표시
        }
        else //플레이어를 한번 더 클릭시, 이동가능거리 표시제거
        {
            if (csTileNum % 8 != 0) tileList[csTileNum].GetComponent<Tile>().enter(false); //우측표시 제거
            if ((csTileNum - 1) % 8 != 0) tileList[csTileNum - 2].GetComponent<Tile>().enter(false); //좌측표시 제거
            if (csTileNum <= 56) tileList[csTileNum + 7].GetComponent<Tile>().enter(false); //하단표시 제거
            if (csTileNum > 8) tileList[csTileNum - 9].GetComponent<Tile>().enter(false); //상단표시 제거
        }
    }

    private void AttackEnemy(GameObject enemy)
    {
        Vector3 playerP = Player.transform.position;
        Vector3 enemyP = enemy.transform.position;

        

        Player.transform.DOMoveX(playerP.x - 0.4f, 1.5f).
            OnComplete(() => Player.transform.DOMoveX(playerP.x + 0.2f, 1f)
            .OnComplete(() => Player.transform.DOMoveX(playerP.x - 0.03f, 0.5f)));

        enemy.transform.DOMoveX(enemyP.x + 0.4f, 1.5f).
            OnComplete(() => enemy.transform.DOMoveX(enemyP.x - 0.2f, 1f)
            .OnComplete(() => enemy.transform.DOMoveX(enemyP.x + 0.03f, 0.5f).
            OnComplete(() => enemy.transform.GetComponent<SpriteRenderer>().DOFade(0, 1f).OnComplete(() => Destroy(enemy)))));
         
    }
}
