using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Security.Authentication;

public class MatchFinder : MonoBehaviour
{
    Board board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsBomb(Chip chip1, Chip chip2, Chip chip3)
    {
        List<GameObject> currentChips = new List<GameObject>();

        if (chip1.isBomb)
        {
            currentMatches.Union(GetAdjacentChips(chip1.column, chip1.row));
        }

        if (chip2.isBomb)
        {
            currentMatches.Union(GetAdjacentChips(chip2.column, chip2.row));
        }

        if (chip3.isBomb)
        {
            currentMatches.Union(GetAdjacentChips(chip3.column, chip3.row));
        }
        return currentChips;
    }

    private List<GameObject> IsRowArrow(Chip chip1, Chip chip2, Chip chip3)
    {
        List<GameObject> currentChips = new List<GameObject>();

        if (chip1.isRowArrow)
        {
            currentMatches.Union(GetRowChips(chip1.row));
        }

        if (chip2.isRowArrow)
        {
            currentMatches.Union(GetRowChips(chip2.row));
        }

        if (chip3.isRowArrow)
        {
            currentMatches.Union(GetRowChips(chip3.row));
        }
        return currentChips;
    }
    
    private List<GameObject> IsColumnArrow(Chip chip1, Chip chip2, Chip chip3)
    {
        List<GameObject> currentChips = new List<GameObject>();

        if (chip1.isColumnArrow)
        {
            currentMatches.Union(GetColumnChips(chip1.column));
        }

        if (chip2.isColumnArrow)
        {
            currentMatches.Union(GetColumnChips(chip2.column));
        }

        if (chip3.isColumnArrow)
        {
            currentMatches.Union(GetColumnChips(chip3.column));
        }
        return currentChips;
    }

    private void AddToListAndMatch(GameObject chip)
    {
        if (!currentMatches.Contains(chip))
        {
            currentMatches.Add(chip);
        }
        chip.GetComponent<Chip>().isMatched = true;
    }
    private void GetNearbyChips(GameObject chip1, GameObject chip2, GameObject chip3)
    {
        AddToListAndMatch(chip1);
        AddToListAndMatch(chip2);
        AddToListAndMatch(chip3);
    }

    IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentChip = board.allChips[i, j];
                if (currentChip != null)
                {
                    Chip currentChipScript = currentChip.GetComponent<Chip>();
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftChip = board.allChips[i - 1, j];
                        GameObject rightChip = board.allChips[i + 1, j];

                        if (leftChip != null && rightChip != null)
                        {
                            Chip leftChipScript = leftChip.GetComponent<Chip>();
                            Chip rightChipScript = rightChip.GetComponent<Chip>();

                            if (leftChip.tag == currentChip.tag && rightChip.tag == currentChip.tag)
                            {
                                currentMatches.Union(IsRowArrow(leftChipScript, currentChipScript, rightChipScript));

                                currentMatches.Union(IsColumnArrow(leftChipScript, currentChipScript, rightChipScript));

                                currentMatches.Union(IsBomb(leftChipScript, currentChipScript, rightChipScript));

                                GetNearbyChips(leftChip, currentChip, rightChip);
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upperChip = board.allChips[i, j + 1];
                        GameObject lowerChip = board.allChips[i, j - 1];

                        if (upperChip != null && lowerChip != null)
                        {
                            Chip upperChipScript = upperChip.GetComponent<Chip>();
                            Chip lowerChipScript = lowerChip.GetComponent<Chip>();

                            if (upperChip.tag == currentChip.tag && lowerChip.tag == currentChip.tag)
                            {
                                currentMatches.Union(IsColumnArrow(upperChipScript, currentChipScript, lowerChipScript));

                                currentMatches.Union(IsRowArrow(upperChipScript, currentChipScript, lowerChipScript));

                                currentMatches.Union(IsBomb(upperChipScript, currentChipScript, lowerChipScript));

                                GetNearbyChips(upperChip, currentChip, lowerChip);
                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchChipsOfColor(string color)
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allChips[i, j] != null && board.allChips[i, j].tag == color)
                {
                    board.allChips[i, j].GetComponent<Chip>().isMatched = true;
                }
            }
        }
    }

    List <GameObject> GetAdjacentChips(int column, int row)
    {
        List<GameObject> chips = new List<GameObject>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                if (i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    if (board.allChips[i, j] != null)
                    {
                        chips.Add(board.allChips[i, j]);
                        board.allChips[i, j].GetComponent<Chip>().isMatched = true;
                    }
                }
            }
        }
        return chips;
    }

    List<GameObject> GetColumnChips(int column)
    {
        List<GameObject> chips = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allChips[column, i] != null)
            {
                Chip chip = board.allChips[column, i].GetComponent<Chip>();
                if (chip.isRowArrow)
                {
                    chips.Union(GetRowChips(i)).ToList();
                }
                chips.Add(board.allChips[column, i]);
                chip.isMatched = true; ;
            }
        }
        return chips;
    }
    
    List<GameObject> GetRowChips(int row)
    {
        List<GameObject> chips = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allChips[i, row] != null)
            {
                Chip chip = board.allChips[i, row].GetComponent<Chip>();
                if (chip.isColumnArrow)
                {
                    chips.Union(GetColumnChips(i)).ToList();
                }
                chips.Add(board.allChips[i, row]);
                chip.isMatched = true; ;
            }
        }
        return chips;
    }

    public void CheckForBoosters()
    {
        if (board.currentChip != null)
        {
            if (board.currentChip.isMatched)
            {
                board.currentChip.isMatched = false;

            if ((board.currentChip.swipeAngle > -45 && board.currentChip.swipeAngle <= 45) ||
                 board.currentChip.swipeAngle > -135 || board.currentChip.swipeAngle <= 135)
                {
                    board.currentChip.MakeRowBomb();
                }
                else
                {
                    board.currentChip.MakeColumnBomb();
                }
            }
            else if (board.currentChip.otherChip != null)
            {
                Chip otherChip = board.currentChip.otherChip.GetComponent<Chip>();
                if (otherChip.isMatched)
                {
                    otherChip.isMatched = false;
                }

                if ((board.currentChip.swipeAngle > -45 && board.currentChip.swipeAngle <= 45) ||
                     board.currentChip.swipeAngle > -135 || board.currentChip.swipeAngle <= 135)
                {
                    otherChip.MakeRowBomb();
                }
                else
                {
                    otherChip.MakeColumnBomb();
                }
            }
        }
    }
}
