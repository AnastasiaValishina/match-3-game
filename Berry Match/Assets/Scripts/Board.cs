using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum TileKind
{
    Breakable,
    Blank,
    Lock,
    Concrete,
    Slime,
    Normal 
}

[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
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
    public int offset = 10;
    public Chip[,] allChips;
    public GameObject destroyEffect;
    public Chip currentChip;
    public int baseChipValue = 5;
    public float refillDelay = 0.5f;
    public MatchType matchType;

    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject lockTilePrefab;
    public GameObject concreteTilePrefab;
    public GameObject slimeTilePrefab;
    
    [Header("Default Level")]
    public World world;
    public int level;
    public int width = 8;
    public int height = 8;
    public Chip[] chips;
    public TileType[] boardLayout;
    public int[] scoreGoals;

    int streakValue = 1;
    bool[,] blankSpaces;
    MatchFinder matchFinder;
    BreakableTile[,] breakableTiles;
    BreakableTile[,] concreteTiles;
    BreakableTile[,] slimeTiles;
    public BreakableTile[,] lockTiles;
    ScoreManager scoreManager;
    SoundManager soundManager;
    GoalManager goalManager;
    bool makeSlime = true;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }

        if (world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null)
                {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    chips = world.levels[level].chips;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                }
            }
        }
    }
    void Start()
    {
        allChips = new Chip[width, height];
        blankSpaces = new bool[width, height];
        breakableTiles = new BreakableTile[width, height];
        concreteTiles = new BreakableTile[width, height];        
        lockTiles = new BreakableTile[width, height];
        slimeTiles = new BreakableTile[width, height];
        matchFinder = FindObjectOfType<MatchFinder>();
        scoreManager = FindObjectOfType<ScoreManager>();
        soundManager = FindObjectOfType<SoundManager>();
        goalManager = FindObjectOfType<GoalManager>();
        SetUp();
        currentState = GameState.pause;
    }

    public void GenerateBlankSpaces()
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
    
    void GenerateLockTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Lock)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BreakableTile>();
            }
        }
    }

    void GenerateConcreteTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Concrete)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(concreteTilePrefab, tempPosition, Quaternion.identity);
                concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BreakableTile>();
            }
        }
    }
    
    void GenerateSlimeTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Slime)
            {
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(slimeTilePrefab, tempPosition, Quaternion.identity);
                slimeTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BreakableTile>();
            }
        }
    }

    void SetUp()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();
        GenerateLockTiles();
        GenerateConcreteTiles();
        GenerateSlimeTiles();

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

                    if (!concreteTiles[i, j] && !slimeTiles[i, j])
                    {
                        Vector2 chipPosition = new Vector2(i, j + offset);
                        int randomChip = Random.Range(0, chips.Length);
                        int maxIterations = 0;
                        while (MatchesAt(i, j, chips[randomChip]) && maxIterations < 100)
                        {
                            randomChip = Random.Range(0, chips.Length);
                            maxIterations++;
                        }
                        maxIterations = 0;
                        Chip chip = Instantiate(chips[randomChip], chipPosition, Quaternion.identity);
                        chip.row = j;
                        chip.column = i;

                        chip.transform.parent = transform;
                        chip.name = "( " + i + ", " + j + " )";
                        allChips[i, j] = chip;
                    }
                }
            }
        }
    }

    bool MatchesAt(int column, int row, Chip chip)
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

    private MatchType ColumnOrRow()
    {
        List<Chip> matchCopy = matchFinder.currentMatches as List<Chip>; // copy of current matches

        matchType.type = 0;
        matchType.color = "";

        for (int i = 0; i < matchCopy.Count; i++)
        {
            Chip thisChip = matchCopy[i];
            string color = matchCopy[i].tag;

            int column = thisChip.column;
            int row = thisChip.row;
            int columnMatch = 0;
            int rowMatch = 0;

            for (int j = 0; j < matchCopy.Count; j++)
            {
                Chip nextChip = matchCopy[j];

                if (nextChip == thisChip)
                {
                    continue;
                }
                if (nextChip.column == thisChip.column && nextChip.tag == color)
                {
                    columnMatch++;
                }
                if (nextChip.row == thisChip.row && nextChip.tag == color)
                {
                    rowMatch++;
                }
            }

            if (columnMatch == 4 || rowMatch == 4)
            {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 2 && rowMatch == 2)
            {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 3 || rowMatch == 3)
            {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }
        }

        matchType.type = 0;
        matchType.color = "";
        return matchType;        
    }

    private void CheckToMakeBombs()
    {       
        if (matchFinder.currentMatches.Count > 3)
        {
            MatchType typeOfMatch = ColumnOrRow();
            if (typeOfMatch.type == 1)
            {
                // make rainbow bomb
                if (currentChip != null && currentChip.isMatched && currentChip.tag == typeOfMatch.color)
                {
                    currentChip.isMatched = false;
                    currentChip.MakeColorBomb();
                }
                else
                {
                    if (currentChip.otherChip != null)
                    {
                        Chip otherChip = currentChip.otherChip;
                        if (otherChip.isMatched && otherChip.tag == typeOfMatch.color)
                        {
                            otherChip.isMatched = false;
                            otherChip.MakeColorBomb();
                        }
                    }
                }                
            }
            else if(typeOfMatch.type == 2)
            {
                // make a bomb
                if (currentChip != null && currentChip.isMatched && currentChip.tag == typeOfMatch.color)
                {
                    currentChip.isMatched = false;
                    currentChip.MakeAdjacentBomb();
                }
                else if (currentChip.otherChip != null)
                {
                    Chip otherChip = currentChip.otherChip;
                    if (otherChip.isMatched && otherChip.tag == typeOfMatch.color)
                    {
                        otherChip.isMatched = false;
                        otherChip.MakeAdjacentBomb();                                
                    }                    
                }
            }
            else if (typeOfMatch.type == 3)
            {
                matchFinder.CheckForBoosters(typeOfMatch);
            }
        }
    }

    public void BombRow(int row)
    {
        for (int i = 0; i < width; i++)
        {
            if (concreteTiles[i, row])
            {
                concreteTiles[i, row].TakeDamage(1);
                if (concreteTiles[i, row].hitPoints <= 0)
                {
                    concreteTiles[i, row] = null;
                }
            }            
        }
    }
    
    public void BombColumn(int colunm)
    {
        for (int i = 0; i < width; i++)
        {
             if (concreteTiles[colunm, i])
             {
                 concreteTiles[colunm, i].TakeDamage(1);
                 if (concreteTiles[colunm, i].hitPoints <= 0)
                 {
                    concreteTiles[colunm, i] = null;
                 }               
            }
        }
    }

    void DestroyMatchesAt(int column, int row)
    {
        if (allChips[column, row].isMatched)
        {
            if (breakableTiles[column, row] != null)
            {
                breakableTiles[column, row].TakeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }
            if (lockTiles[column, row] != null)
            {
                lockTiles[column, row].TakeDamage(1);
                if (lockTiles[column, row].hitPoints <= 0)
                {
                    lockTiles[column, row] = null;
                }
            }
            DamageConcreteTiles(column, row);
            DamageSlimeTiles(column, row);

            if (goalManager != null)
            {
                goalManager.CompareGoal(allChips[column, row].tag.ToString());
                goalManager.UpdateGoals();
                
            }

            if (soundManager != null)
            {
                soundManager.PlayRandomDestroyNoise();
            }

            GameObject particle = Instantiate(destroyEffect, allChips[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 1f);
            // Play animation from chip script
            Destroy(allChips[column, row].gameObject);
            scoreManager.IncreaseScore(baseChipValue * streakValue);
            allChips[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        if (matchFinder.currentMatches.Count >= 4)
        {
            CheckToMakeBombs();
        }
        matchFinder.currentMatches.Clear();

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

    void DamageConcreteTiles(int column, int row)
    {
        if (column > 0)
        {
            if (concreteTiles[column - 1, row])
            {
                concreteTiles[column - 1, row].TakeDamage(1);
                if (concreteTiles[column - 1, row].hitPoints <= 0)
                {
                    concreteTiles[column - 1, row] = null;
                }
            }
        }
        if (column < width - 1)
        {
            if (concreteTiles[column + 1, row])
            {
                concreteTiles[column + 1, row].TakeDamage(1);
                if (concreteTiles[column + 1, row].hitPoints <= 0)
                {
                    concreteTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            if (concreteTiles[column, row - 1])
            {
                concreteTiles[column, row - 1].TakeDamage(1);
                if (concreteTiles[column, row - 1].hitPoints <= 0)
                {
                    concreteTiles[column, row - 1] = null;
                }
            }
        }
        if (row < height - 1)
        {
            if (concreteTiles[column, row + 1])
            {
                concreteTiles[column, row + 1].TakeDamage(1);
                if (concreteTiles[column, row + 1].hitPoints <= 0)
                {
                    concreteTiles[column, row + 1] = null;
                }
            }
        }
    }
    
    void DamageSlimeTiles(int column, int row)
    {
        if (column > 0)
        {
            if (slimeTiles[column - 1, row])
            {
                slimeTiles[column - 1, row].TakeDamage(1);
                if (slimeTiles[column - 1, row].hitPoints <= 0)
                {
                    slimeTiles[column - 1, row] = null;
                }
                makeSlime = false;
            }
        }
        if (column < width - 1)
        {
            if (slimeTiles[column + 1, row])
            {
                slimeTiles[column + 1, row].TakeDamage(1);
                if (slimeTiles[column + 1, row].hitPoints <= 0)
                {
                    slimeTiles[column + 1, row] = null;
                }
                makeSlime = false;
            }
        }
        if (row > 0)
        {
            if (slimeTiles[column, row - 1])
            {
                slimeTiles[column, row - 1].TakeDamage(1);
                if (slimeTiles[column, row - 1].hitPoints <= 0)
                {
                    slimeTiles[column, row - 1] = null;
                }
                makeSlime = false;
            }
        }
        if (row < height - 1)
        {
            if (slimeTiles[column, row + 1])
            {
                slimeTiles[column, row + 1].TakeDamage(1);
                if (slimeTiles[column, row + 1].hitPoints <= 0)
                {
                    slimeTiles[column, row + 1] = null;
                }
                makeSlime = false;
            }
        }
    }

    IEnumerator DecreaseRow()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && allChips[i, j] == null && !concreteTiles[i, j] && !slimeTiles[i, j])
                {
                    for (int k = j + 1; k < height; k++)
                    {
                        if (allChips[i, k] != null)
                        {
                            allChips[i, k].row = j;
                            allChips[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoard());
    }

    void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChips[i, j] == null && !blankSpaces[i, j] && !concreteTiles[i, j] && !slimeTiles[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    int randomChip = Random.Range(0, chips.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, chips[randomChip]) && maxIterations < 100)
                    {
                        maxIterations++;
                        randomChip = Random.Range(0, chips.Length);
                    }
                    maxIterations = 0;
                    Chip chip = Instantiate(chips[randomChip], tempPosition, Quaternion.identity);
                    chip.transform.parent = transform;
                    allChips[i, j] = chip;
                    chip.row = j;
                    chip.column = i;
                }
            }
        }
    }

    bool MatchesOnBoard()
    {
        matchFinder.FindAllMatches();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allChips[i, j] != null)
                {
                    if (allChips[i, j].isMatched)
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
        yield return new WaitForSeconds(refillDelay);
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);
        while (MatchesOnBoard())
        {
            streakValue++;
            DestroyMatches();
          //  yield return new WaitForSeconds(2 * refillDelay);
            yield break;
        }
        CheckToMakeSlime();
        currentChip = null;

        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
        yield return new WaitForSeconds(refillDelay);
        if (currentState != GameState.pause)
        {
            currentState = GameState.move;
        }
        makeSlime = true;
        streakValue = 1;
    }

    void CheckToMakeSlime()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (slimeTiles[i, j] != null && makeSlime)
                {
                    MakeNewSlime();
                    return;
                }
            }
        }
    }

    Vector2 CheckForAdjacent(int column, int row)
    {
        if (column < width - 1 && allChips[column + 1, row])
        {
            return Vector2.right;
        }
        if (column > 0 && allChips[column - 1, row])
        {
            return Vector2.left;
        }
        if (row < height - 1 && allChips[column, row + 1])
        {
            return Vector2.up;
        }
        if (row > 0 && allChips[column, row - 1])
        {
            return Vector2.down;
        }
        return Vector2.zero;
    }

    void MakeNewSlime()
    {
        bool slime = false;
        int loops = 0;
        while (!slime && loops < (width * height))
        {
            int newX = Random.Range(0, width);
            int newY = Random.Range(0, height);
            if (slimeTiles[newX, newY])
            {
                Vector2 adjacent = CheckForAdjacent(newX, newY);
                if (adjacent != Vector2.zero)
                {
                    Destroy(allChips[newX + (int)adjacent.x, newY + (int)adjacent.y]);
                    Vector2 tempPosition = new Vector2(newX + (int)adjacent.x, newY + (int)adjacent.y);
                    GameObject tile = Instantiate(slimeTilePrefab, tempPosition, Quaternion.identity);
                    slimeTiles[newX + (int)adjacent.x, newY + (int)adjacent.y] = tile.GetComponent<BreakableTile>(); 
                    slime = true;
                }
            }
            loops++;
        }
    }

    void SwitchChips(int column, int row, Vector2 direction)
    {
        if (allChips[column + (int)direction.x, row + (int)direction.y] != null)
        {
            Chip holder = allChips[column + (int)direction.x, row + (int)direction.y];
            allChips[column + (int)direction.x, row + (int)direction.y] = allChips[column, row];
            allChips[column, row] = holder;
        }
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

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
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
        List<Chip> currentBoard = new List<Chip>();

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
                if (!blankSpaces[i, j] && !concreteTiles[i, j] && !slimeTiles[i, j])
                {
                    int randomChip = Random.Range(0, currentBoard.Count);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, currentBoard[randomChip]) && maxIterations < 100)
                    {
                        randomChip = Random.Range(0, currentBoard.Count);
                        maxIterations++;
                    }
                    maxIterations = 0;
                    Chip chip = currentBoard[randomChip];
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
