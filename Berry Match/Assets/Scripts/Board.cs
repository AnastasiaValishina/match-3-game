using System.Collections;
using System.Data;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width = 8;
    public int height = 8;
    public int offset = 10;
    public GameObject tilePrefab;
    public GameObject[] chips;
    public GameObject[,] allChips;
    public GameObject destroyEffect;
    public Chip currentChip;


    BackgroundTile[,] allTiles;
    MatchFinder matchFinder;

    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allChips = new GameObject[width, height];
        matchFinder = FindObjectOfType<MatchFinder>();
        SetUp();
    }

    void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tilePosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                Vector2 chipPosition = new Vector2(i, j + offset);
                int randomChip = Random.Range(0, chips.Length);
                int maxIterations = 0;
                while (MatchesAt(i, j, chips[randomChip]) && maxIterations < 100)
                {
                    randomChip = Random.Range(0, chips.Length);
                    maxIterations++;
                }
                maxIterations = 0;
                GameObject chip = Instantiate(chips[randomChip], chipPosition, Quaternion.identity);
                chip.GetComponent<Chip>().row = j;
                chip.GetComponent<Chip>().column = i;

                chip.transform.parent = transform;
                chip.name = "( " + i + ", " + j + " )";
                allChips[i, j] = chip;
            } 
        }
    }

    bool MatchesAt(int column, int row, GameObject chip)
    {
        if (column > 1 && row > 1)
        {
            if (allChips[column - 1, row].tag == chip.tag && allChips[column - 2, row].tag == chip.tag)
            {
                return true;
            }
            if (allChips[column, row - 1].tag == chip.tag && allChips[column, row - 2].tag == chip.tag)
            {
                return true;
            }
        } 
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if(allChips[column, row -1].tag == chip.tag && allChips[column, row - 2].tag == chip.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if(allChips[column - 1, row].tag == chip.tag && allChips[column - 2, row].tag == chip.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void DestroyMatchesAt(int column, int row)
    {
        if (allChips[column, row].GetComponent<Chip>().isMatched)
        {
            if (matchFinder.currentMatches.Count == 4 || matchFinder.currentMatches.Count == 7)
            {
                matchFinder.CheckForBoosters();
            }

            matchFinder.currentMatches.Remove(allChips[column, row]);
            GameObject particle = Instantiate(destroyEffect, allChips[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 1f);
            Destroy(allChips[column, row]);
            allChips[column, row] = null;
        } 
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChips[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRow());
    }

    IEnumerator DecreaseRow()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChips[i, j] == null)
                {
                    nullCount++;
                } 
                else if (nullCount > 0)
                {
                    allChips[i, j].GetComponent<Chip>().row -= nullCount;
                    allChips[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FillBoard());
    }

    void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChips[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    int randomChip = Random.Range(0, chips.Length);
                    GameObject chip = Instantiate(chips[randomChip], tempPosition, Quaternion.identity);
                    allChips[i, j] = chip;
                    chip.GetComponent<Chip>().row = j;
                    chip.GetComponent<Chip>().column = i;
                }
            }
        }
    }

    bool MatchesOnBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChips[i, j] != null)
                {
                    if(allChips[i, j].GetComponent<Chip>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }

        matchFinder.currentMatches.Clear();
        currentChip = null;
        yield return new WaitForSeconds(0.5f);
        currentState = GameState.move;
    }
}
