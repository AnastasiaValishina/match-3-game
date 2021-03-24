using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
