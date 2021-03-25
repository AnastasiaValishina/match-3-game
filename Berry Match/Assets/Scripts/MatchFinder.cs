using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftChip = board.allChips[i - 1, j];
                        GameObject rightChip = board.allChips[i + 1, j];
                        if (leftChip != null && rightChip != null)
                        {
                            if (leftChip.tag == currentChip.tag && rightChip.tag == currentChip.tag)
                            {
                                if (currentChip.GetComponent<Chip>().isRowArrow || 
                                    leftChip.GetComponent<Chip>().isRowArrow ||
                                    rightChip.GetComponent<Chip>().isRowArrow)
                                {
                                    currentMatches.Union(GetRowChips(j));
                                }

                                if (currentChip.GetComponent<Chip>().isColumnArrow)
                                {
                                    currentMatches.Union(GetColumnChips(i));
                                }

                                if (leftChip.GetComponent<Chip>().isColumnArrow)
                                {
                                    currentMatches.Union(GetColumnChips(i - 1));
                                }

                                if (rightChip.GetComponent<Chip>().isColumnArrow)
                                {
                                    currentMatches.Union(GetColumnChips(i + 1));
                                }


                                if (!currentMatches.Contains(leftChip))
                                {
                                    currentMatches.Add(leftChip);
                                }
                                leftChip.GetComponent<Chip>().isMatched = true;

                                if (!currentMatches.Contains(rightChip))
                                {
                                    currentMatches.Add(rightChip);
                                }
                                rightChip.GetComponent<Chip>().isMatched = true;

                                if (!currentMatches.Contains(currentChip))
                                {
                                    currentMatches.Add(currentChip);
                                }
                                currentChip.GetComponent<Chip>().isMatched = true;
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upperChip = board.allChips[i, j + 1];
                        GameObject lowerChip = board.allChips[i, j - 1];
                        if (upperChip != null && lowerChip != null)
                        {
                            if (upperChip.tag == currentChip.tag && lowerChip.tag == currentChip.tag)
                            {
                                if (currentChip.GetComponent<Chip>().isColumnArrow ||
                                    upperChip.GetComponent<Chip>().isColumnArrow ||
                                    lowerChip.GetComponent<Chip>().isColumnArrow)
                                {
                                    currentMatches.Union(GetColumnChips(i));
                                }

                                if (currentChip.GetComponent<Chip>().isRowArrow)
                                {
                                    currentMatches.Union(GetRowChips(j));
                                }

                                if (upperChip.GetComponent<Chip>().isRowArrow)
                                {
                                    currentMatches.Union(GetRowChips(j + 1));
                                }

                                if (lowerChip.GetComponent<Chip>().isRowArrow)
                                {
                                    currentMatches.Union(GetRowChips(j - 1));
                                }


                                if (!currentMatches.Contains(upperChip))
                                {
                                    currentMatches.Add(upperChip);
                                }
                                upperChip.GetComponent<Chip>().isMatched = true;

                                if (!currentMatches.Contains(lowerChip))
                                {
                                    currentMatches.Add(lowerChip);
                                }
                                lowerChip.GetComponent<Chip>().isMatched = true;

                                if (!currentMatches.Contains(currentChip))
                                {
                                    currentMatches.Add(currentChip);
                                }
                                currentChip.GetComponent<Chip>().isMatched = true;

                            }
                        }
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnChips(int column)
    {
        List<GameObject> chips = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allChips[column, i] != null)
            {
                chips.Add(board.allChips[column, i]);
                board.allChips[column, i].GetComponent<Chip>().isMatched = true; ;
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
                chips.Add(board.allChips[i, row]);
                board.allChips[i, row].GetComponent<Chip>().isMatched = true; ;
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
