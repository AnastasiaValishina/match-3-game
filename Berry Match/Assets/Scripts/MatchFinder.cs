using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviour
{
    public List<Chip> currentMatches = new List<Chip>();
    static MatchFinder instance;

    public static MatchFinder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MatchFinder>();
            }
            return instance;
        }
    }


    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<Chip> IsBomb(Chip chip1, Chip chip2, Chip chip3)
    {
        List<Chip> currentChips = new List<Chip>();

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

    private List<Chip> IsRowArrow(Chip chip1, Chip chip2, Chip chip3)
    {
        List<Chip> currentChips = new List<Chip>();

        if (chip1.isRowArrow)
        {
            currentMatches.Union(GetRowChips(chip1.row));
            Board.Instance.BombRow(chip1.row);
        }

        if (chip2.isRowArrow)
        {
            currentMatches.Union(GetRowChips(chip2.row));
            Board.Instance.BombRow(chip2.row);
        }

        if (chip3.isRowArrow)
        {
            currentMatches.Union(GetRowChips(chip3.row));
            Board.Instance.BombRow(chip3.row);
        }
        return currentChips;
    }
    
    private List<Chip> IsColumnArrow(Chip chip1, Chip chip2, Chip chip3)
    {
        List<Chip> currentChips = new List<Chip>();

        if (chip1.isColumnArrow)
        {
            currentMatches.Union(GetColumnChips(chip1.column));
            Board.Instance.BombColumn(chip1.column);
        }

        if (chip2.isColumnArrow)
        {
            currentMatches.Union(GetColumnChips(chip2.column));
            Board.Instance.BombColumn(chip2.column);
        }

        if (chip3.isColumnArrow)
        {
            currentMatches.Union(GetColumnChips(chip3.column));
            Board.Instance.BombColumn(chip2.column);
        }
        return currentChips;
    }

    private void AddToListAndMatch(Chip chip)
    {
        if (!currentMatches.Contains(chip))
        {
            currentMatches.Add(chip);
        }
        chip.isMatched = true;
    }
    private void GetNearbyChips(Chip chip1, Chip chip2, Chip chip3)
    {
        AddToListAndMatch(chip1);
        AddToListAndMatch(chip2);
        AddToListAndMatch(chip3);
    }

    IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < Board.Instance.width; i++)
        {
            for (int j = 0; j < Board.Instance.height; j++)
            {
                Chip currentChip = Board.Instance.allChips[i, j];
                if (currentChip != null)
                {
                    if (i > 0 && i < Board.Instance.width - 1)
                    {
                        Chip leftChip = Board.Instance.allChips[i - 1, j];
                        Chip rightChip = Board.Instance.allChips[i + 1, j];

                        if (leftChip != null && rightChip != null)
                        {
                            if (leftChip.tag == currentChip.tag && rightChip.tag == currentChip.tag)
                            {
                                currentMatches.Union(IsRowArrow(leftChip, currentChip, rightChip));

                                currentMatches.Union(IsColumnArrow(leftChip, currentChip, rightChip));

                                currentMatches.Union(IsBomb(leftChip, currentChip, rightChip));

                                GetNearbyChips(leftChip, currentChip, rightChip);
                            }
                        }
                    }
                    if (j > 0 && j < Board.Instance.height - 1)
                    {
                        Chip upperChip = Board.Instance.allChips[i, j + 1];
                        Chip lowerChip = Board.Instance.allChips[i, j - 1];

                        if (upperChip != null && lowerChip != null)
                        {
                            if (upperChip.tag == currentChip.tag && lowerChip.tag == currentChip.tag)
                            {
                                currentMatches.Union(IsColumnArrow(upperChip, currentChip, lowerChip));

                                currentMatches.Union(IsRowArrow(upperChip, currentChip, lowerChip));

                                currentMatches.Union(IsBomb(upperChip, currentChip, lowerChip));

                                GetNearbyChips(upperChip, currentChip, lowerChip);
                            }
                        }
                    }
                }
            }
        }
        yield return null;
    }

    public void MatchChipsOfColor(string color)
    {
        for (int i = 0; i < Board.Instance.width; i++)
        {
            for (int j = 0; j < Board.Instance.height; j++)
            {
                if (Board.Instance.allChips[i, j] != null && Board.Instance.allChips[i, j].tag == color)
                {
                    Board.Instance.allChips[i, j].isMatched = true;
                }
            }
        }
    }

    List <Chip> GetAdjacentChips(int column, int row)
    {
        List<Chip> chips = new List<Chip>();
        for (int i = column - 1; i <= column + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                if (i >= 0 && i < Board.Instance.width && j >= 0 && j < Board.Instance.height)
                {
                    if (Board.Instance.allChips[i, j] != null)
                    {
                        chips.Add(Board.Instance.allChips[i, j]);
                        Board.Instance.allChips[i, j].isMatched = true;
                    }
                }
            }
        }
        return chips;
    }

    List<Chip> GetColumnChips(int column)
    {
        List<Chip> chips = new List<Chip>();
        for (int i = 0; i < Board.Instance.height; i++)
        {
            if (Board.Instance.allChips[column, i] != null)
            {
                Chip chip = Board.Instance.allChips[column, i];
                if (chip.isRowArrow)
                {
                    chips.Union(GetRowChips(i)).ToList();
                }
                chips.Add(Board.Instance.allChips[column, i]);
                chip.isMatched = true; ;
            }
        }
        return chips;
    }
    
    List<Chip> GetRowChips(int row)
    {
        List<Chip> chips = new List<Chip>();
        for (int i = 0; i < Board.Instance.width; i++)
        {
            if (Board.Instance.allChips[i, row] != null)
            {
                Chip chip = Board.Instance.allChips[i, row];
                if (chip.isColumnArrow)
                {
                    chips.Union(GetColumnChips(i)).ToList();
                }
                chips.Add(Board.Instance.allChips[i, row]);
                chip.isMatched = true; ;
            }
        }
        return chips;
    }

    public void CheckForBoosters(MatchType matchType)
    {
        if (Board.Instance.currentChip != null)
        {
            if (Board.Instance.currentChip.isMatched && Board.Instance.currentChip.tag == matchType.color)
            {
                Board.Instance.currentChip.isMatched = false;

            if ((Board.Instance.currentChip.swipeAngle > -45 && Board.Instance.currentChip.swipeAngle <= 45) ||
                 Board.Instance.currentChip.swipeAngle > -135 || Board.Instance.currentChip.swipeAngle <= 135)
                {
                    Board.Instance.currentChip.MakeRowBomb();
                }
                else
                {
                    Board.Instance.currentChip.MakeColumnBomb();
                }
            }
            else if (Board.Instance.currentChip.otherChip != null)
            {
                Chip otherChip = Board.Instance.currentChip.otherChip;
                if (otherChip.isMatched && otherChip.tag == matchType.color)
                {
                    otherChip.isMatched = false;
                }

                if ((Board.Instance.currentChip.swipeAngle > -45 && Board.Instance.currentChip.swipeAngle <= 45) ||
                     Board.Instance.currentChip.swipeAngle > -135 || Board.Instance.currentChip.swipeAngle <= 135)
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
