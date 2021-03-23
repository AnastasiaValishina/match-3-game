using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public GameObject tilePrefab;
    public GameObject[] chips;
    public GameObject[,] allChips;

    BackgroundTile[,] allTiles; 

    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allChips = new GameObject[width, height];
        SetUp();
    }

    void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                int randomChip = Random.Range(0, chips.Length);
                int maxIterations = 0;
                while (MatchesAt(i, j, chips[randomChip]) && maxIterations < 100)
                {
                    randomChip = Random.Range(0, chips.Length);
                    maxIterations++;
                }
                maxIterations = 0;
                GameObject chip = Instantiate(chips[randomChip], tempPosition, Quaternion.identity);
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
}
