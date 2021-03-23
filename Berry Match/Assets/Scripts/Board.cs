using System.Collections;
using System.Collections.Generic;
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
                GameObject chip = Instantiate(chips[randomChip], tempPosition, Quaternion.identity);
                chip.transform.parent = transform;
                chip.name = "( " + i + ", " + j + " )";
                allChips[i, j] = chip;
            } 
        }
    }
}
