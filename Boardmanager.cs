using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Boardmanager : MonoBehaviour
{
    public GameObject[] tilePrefabs;
    public int boardWidth = 6;
    public int boardHeight = 8;
    public int score = 0;
    public int lives = 3;
    public int steps = 20;
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI livesText;
    public TMPro.TextMeshProUGUI stepsText;
    private GameObject[,] tiles; 
    private Tile firstSelected = null;
    void Start()
    {
        tiles = new GameObject[boardWidth, boardHeight];
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                int idx = Random.Range(0, tilePrefabs.Length);
                float offsetX = (boardWidth - 1) / 2f;
                float offsetY = (boardHeight - 1) / 2f;
                Vector3 pos = new Vector3(x - offsetX, y - offsetY, 0);
                GameObject tileObj = Instantiate(tilePrefabs[idx], pos, Quaternion.identity);
                tileObj.transform.parent = this.transform;
                tiles[x, y] = tileObj;
                Tile tileScript = tileObj.AddComponent<Tile>();
                tileScript.Init(x, y, this);
                tileScript.type = idx;
            }
        }

        // 检查初始棋盘是否有可消除的情况，若有则自动消除
        List<Tile> matched = CheckMatches();
        if (matched.Count > 0)
        {
            StartCoroutine(DestroyAndDrop(matched));
        }
    }
public void AddScore(int points)
    {
        score += points;
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        Debug.Log("Current Score: " + score);
    }
public void GameOver()
    {
        PlayerPrefs.SetInt("FinalScore", score);
        Application.LoadLevel("GameOver");
    }
public void OnTileClicked(Tile tile)
    {
        if (firstSelected == null)
        {
            firstSelected = tile;

        }
        else
        {
            // 判断是否相邻
            if ((Mathf.Abs(tile.x - firstSelected.x) == 1 && tile.y == firstSelected.y) ||
                (Mathf.Abs(tile.y - firstSelected.y) == 1 && tile.x == firstSelected.x))
            {
                SwapTiles(firstSelected, tile);
            }
            firstSelected = null;
        }
    }
    void SwapTiles(Tile a, Tile b)
    {
        // 交换tiles数组
        GameObject temp = tiles[a.x, a.y];
        tiles[a.x, a.y] = tiles[b.x, b.y];
        tiles[b.x, b.y] = temp;

        // 交换位置
        Vector3 posA = a.transform.position;
        a.transform.position = b.transform.position;
        b.transform.position = posA;

        // 交换Tile的x,y
        int tx = a.x, ty = a.y;
        a.x = b.x; a.y = b.y;
        b.x = tx; b.y = ty;

        // 检查是否有消除
        List<Tile> matched = CheckMatches();
        if (matched.Count > 0)
        {
            StartCoroutine(DestroyAndDrop(matched));
            steps--;
            stepsText.text = "Steps: " + Mathf.Max(steps, 0);
            if (steps <= 0)
            {
                GameOver();
            }
        }
        else
        {
            if (lives <= 0)
            {
                // 交换tiles数组
                GameObject tempx = tiles[a.x, a.y];
                tiles[a.x, a.y] = tiles[b.x, b.y];
                tiles[b.x, b.y] = tempx;

                // 交换位置
                Vector3 posAb = a.transform.position;
                a.transform.position = b.transform.position;
                b.transform.position = posAb;

                // 交换Tile的x,y
                int txx = a.x, tyy = a.y;
                a.x = b.x; a.y = b.y;
                b.x = txx; b.y = tyy;
            }
            lives--;
            livesText.text = "Chances: " + Mathf.Max(lives, 0);
        }
    }
// 检查所有横向和纵向的3连及以上棋子，返回要消除的Tile列表
public List<Tile> CheckMatches()
{
    List<Tile> matched = new List<Tile>();
    HashSet<Tile> processed = new HashSet<Tile>();
    // 横向
    for (int y = 0; y < boardHeight; y++)
    {
        int count = 1;
        for (int x = 1; x < boardWidth; x++)
        {
            if (tiles[x, y] == null || tiles[x - 1, y] == null) // 增加判空
            {
                count = 1;
                continue;
            }
            Tile curr = tiles[x, y].GetComponent<Tile>();
            Tile prev = tiles[x - 1, y].GetComponent<Tile>();
            if (curr.type == prev.type)
            {
                count++;
                    if (x == boardWidth - 1 && count >= 3)
                    {
                        for (int k = 0; k < count; k++)
                            matched.Add(tiles[x - k, y].GetComponent<Tile>());
                        AddScore(10*(int)Mathf.Pow(count-2, 2));
                }
            }
            else
            {
                    if (count >= 3)
                    {
                        for (int k = 1; k <= count; k++)
                            matched.Add(tiles[x - k, y].GetComponent<Tile>());
                        AddScore(10*(int)Mathf.Pow(count-2, 2)); // 增加分数
                }   
                count = 1;
            }
        }
    }

    // 纵向
    for (int x = 0; x < boardWidth; x++)
    {
        int count = 1;
        for (int y = 1; y < boardHeight; y++)
        {
            if (tiles[x, y] == null || tiles[x, y - 1] == null) // 增加判空
            {
                count = 1;
                continue;
            }
            Tile curr = tiles[x, y].GetComponent<Tile>();
            Tile prev = tiles[x, y - 1].GetComponent<Tile>();
            if (curr.type == prev.type)
            {
                count++;
                    if (y == boardHeight - 1 && count >= 3)
                    {
                        for (int k = 0; k < count; k++)
                            matched.Add(tiles[x, y - k].GetComponent<Tile>());
                        AddScore(10*(int)Mathf.Pow(count-2, 2));
                }
            }
            else
            {
                    if (count >= 3)
                    {
                        for (int k = 1; k <= count; k++)
                            matched.Add(tiles[x, y - k].GetComponent<Tile>());
                        AddScore(10*(int)Mathf.Pow(count-2, 2)); // 增加分数
                }
                count = 1;
            }
        }
    }

    return matched;
}
    private IEnumerator DestroyAndDrop(List<Tile> matched)
    {
        // 1. 消除棋子
        foreach (Tile t in matched)
        {
            Destroy(tiles[t.x, t.y]);
            tiles[t.x, t.y] = null;
        }


        yield return new WaitForSeconds(0.2f); // 可加消除动画

        // 2. 下落补充
        for (int x = 0; x < boardWidth; x++)
        {
            int emptyCount = 0;
            for (int y = 0; y < boardHeight; y++)
            {
                if (tiles[x, y] == null)
                {
                    emptyCount++;
                }
                else if (emptyCount > 0)
                {
                    // 下落
                    tiles[x, y - emptyCount] = tiles[x, y];
                    tiles[x, y - emptyCount].transform.position += Vector3.down * emptyCount;
                    Tile tileScript = tiles[x, y - emptyCount].GetComponent<Tile>();
                    tileScript.y = y - emptyCount;
                    tiles[x, y] = null;
                }
            }
            // 顶部补新棋子
            for (int i = 0; i < emptyCount; i++)
            {
                int idx = Random.Range(0, tilePrefabs.Length);
                float offsetX = (boardWidth - 1) / 2f;
                float offsetY = (boardHeight - 1) / 2f;
                int y = boardHeight - 1 - i;
                Vector3 pos = new Vector3(x - offsetX, y - offsetY, 0);
                GameObject tileObj = Instantiate(tilePrefabs[idx], pos, Quaternion.identity);
                tileObj.transform.parent = this.transform;
                Tile tileScript = tileObj.AddComponent<Tile>();
                tileScript.Init(x, y, this);
                tileScript.type = idx;
                tiles[x, y] = tileObj;
            }
        }

        yield return new WaitForSeconds(0.2f);

        // 3. 检查是否还有新的消除
        List<Tile> newMatched = CheckMatches();
        if (newMatched.Count > 0)
        {
            StartCoroutine(DestroyAndDrop(newMatched));
        }

    }
    void Update()
    {
        
    }
}
