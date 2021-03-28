using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal

}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width = 8;
    public int height = 8;
    public int offset = 10;
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject[] chips;
    public GameObject[,] allChips;
    public GameObject destroyEffect;
    public Chip currentChip;
    public TileType[] boardLayout;

    bool[,] blankSpaces;
    MatchFinder matchFinder;
    BreakableTile[,] breakableTiles;

    void Start()
    {
        allChips = new GameObject[width, height];
        blankSpaces = new bool[width, height];
        breakableTiles = new BreakableTile[width, height];
        matchFinder = FindObjectOfType<MatchFinder>();
        SetUp();
    }

    public void GenerateBlanckSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakableTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BreakableTile>();
            }
        }
    }

    void SetUp()
    {
        GenerateBlanckSpaces();
        GenerateBreakableTiles();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
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
    }

    bool MatchesAt(int column, int row, GameObject chip)
    {
        if (column > 1 && row > 1)
        {
            if (allChips[column - 1, row] != null && allChips[column - 2, row] != null)
            {
                if (allChips[column - 1, row].tag == chip.tag && allChips[column - 2, row].tag == chip.tag)
                {
                    return true;
                }
            }

            if (allChips[column, row - 1] != null && allChips[column, row - 2] != null)
            {
                if (allChips[column, row - 1].tag == chip.tag && allChips[column, row - 2].tag == chip.tag)
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allChips[column, row - 1] != null && allChips[column, row - 2] != null)
                {
                    if (allChips[column, row - 1].tag == chip.tag && allChips[column, row - 2].tag == chip.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (allChips[column - 1, row] != null && allChips[column - 2, row] != null)
                {
                    if (allChips[column - 1, row].tag == chip.tag && allChips[column - 2, row].tag == chip.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Chip firstChip = matchFinder.currentMatches[0].GetComponent<Chip>();

        if (firstChip != null)
        {
            foreach (GameObject currentChip in matchFinder.currentMatches)
            {
                Chip chip = currentChip.GetComponent<Chip>();
                if (chip.row == firstChip.row)
                {
                    numberHorizontal++;
                }
                if (chip.column == firstChip.column)
                {
                    numberVertical++;
                }
            }
        }
        return numberVertical == 5 || numberHorizontal == 5;
    }

    private void CheckToMakeBombs()
    {
        if (matchFinder.currentMatches.Count == 4 || matchFinder.currentMatches.Count == 7)
        {
            matchFinder.CheckForBoosters();
        }

        if (matchFinder.currentMatches.Count == 5 || matchFinder.currentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                // make rainbow bomb
                if (currentChip != null)
                {
                    if (currentChip.isMatched)
                    {
                        if (!currentChip.isColorBomb)
                        {
                            currentChip.isMatched = false;
                            currentChip.MakeColorBomb();
                        }
                    }
                    else
                    {
                        if (currentChip.otherChip != null)
                        {
                            Chip otherChip = currentChip.otherChip.GetComponent<Chip>();
                            if (otherChip.isMatched)
                            {
                                if (!otherChip.isColorBomb)
                                {
                                    otherChip.isMatched = false;
                                    otherChip.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // make a bomb
                if (currentChip != null)
                {
                    if (currentChip.isMatched)
                    {
                        if (!currentChip.isBomb)
                        {
                            currentChip.isMatched = false;
                            currentChip.MakeAdjacentBomb();
                        }
                    }
                    else
                    {
                        if (currentChip.otherChip != null)
                        {
                            Chip otherChip = currentChip.otherChip.GetComponent<Chip>();
                            if (otherChip.isMatched)
                            {
                                if (!otherChip.isBomb)
                                {
                                    otherChip.isMatched = false;
                                    otherChip.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void DestroyMatchesAt(int column, int row)
    {
        if (allChips[column, row].GetComponent<Chip>().isMatched)
        {
            if (matchFinder.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }

            if (breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }

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
        matchFinder.currentMatches.Clear();
        StartCoroutine(DecreaseRow2());
    }

    IEnumerator DecreaseRow2()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && allChips[i, j] == null)
                {
                    for (int k = j + 1; k < height; k++)
                    {
                        if (allChips[i, k] != null)
                        {
                            allChips[i, k].GetComponent<Chip>().row = j;
                            allChips[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FillBoard());
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
                if (allChips[i, j] == null && !blankSpaces[i, j])
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
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChips[i, j] != null)
                {
                    if (allChips[i, j].GetComponent<Chip>().isMatched)
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

        if (IsDeadLocked())
        {
            ShuffleBoard();
        }

        currentState = GameState.move;
    }

    void SwitchChips(int column, int row, Vector2 direction)
    {
        GameObject holder = allChips[column + (int)direction.x, row + (int)direction.y] as GameObject;
        allChips[column + (int)direction.x, row + (int)direction.y] = allChips[column, row];
        allChips[column, row] = holder;
    }

    bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChips[i, j] != null)
                {
                    if (i < width - 2)
                    {
                        if (allChips[i + 1, j] != null && allChips[i + 2, j] != null)
                        {
                            if (allChips[i + 1, j].tag == allChips[i, j].tag && allChips[i + 2, j].tag == allChips[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        if (allChips[i, j + 1] != null && allChips[i, j + 2] != null)
                        {
                            if (allChips[i, j + 1].tag == allChips[i, j].tag && allChips[i, j + 2].tag == allChips[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchChips(column, row, direction);
        if (CheckForMatches())
        {
            SwitchChips(column, row, direction);
            return true;
        }
        SwitchChips(column, row, direction);
        return false;
    }

    bool IsDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChips[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;        
    }

    void ShuffleBoard()
    {
        List<GameObject> currentBoard = new List<GameObject>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChips[i, j] != null)
                {
                    currentBoard.Add(allChips[i, j]);
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    int randomChip = Random.Range(0, currentBoard.Count);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, currentBoard[randomChip]) && maxIterations < 100)
                    {
                        randomChip = Random.Range(0, currentBoard.Count);
                        maxIterations++;
                    }
                    maxIterations = 0;
                    Chip chip = currentBoard[randomChip].GetComponent<Chip>();
                    chip.column = i;
                    chip.row = j;
                    allChips[i, j] = currentBoard[randomChip];
                    currentBoard.Remove(currentBoard[randomChip]);
                }
            }                
        }
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
    }
}
