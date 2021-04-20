using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    [SerializeField] float hintDelay = 5f; 
    [SerializeField] GameObject hintParticle;
    
    Board board;
    float hintDelaySeconds;
    public GameObject currentHint;

    void Start()
    {
        board = FindObjectOfType<Board>();
        hintDelaySeconds = hintDelay;
    }

    void Update()
    {
        hintDelaySeconds -= Time.deltaTime;
        if (hintDelaySeconds <= 0 && currentHint == null)
        {
            HighlightHint();
            hintDelaySeconds = hintDelay;
        }
    }

    List<Chip> FindAllMatches()
    {
        List<Chip> possibleMoves = new List<Chip>();
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allChips[i, j] != null)
                {
                    if (i < board.width - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
                            possibleMoves.Add(board.allChips[i, j]);
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
                            possibleMoves.Add(board.allChips[i, j]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    Chip GetRandomMatch()
    {
        List<Chip> possibleMoves = new List<Chip>();
        possibleMoves = FindAllMatches();
        if (possibleMoves.Count > 0)
        {
            int randomChip = Random.Range(0, possibleMoves.Count);
            return possibleMoves[randomChip];
        }
        return null;
    }
    void HighlightHint()
    {
        Chip move = GetRandomMatch();
        if (move != null)
        {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }
    public void DestroyHint()
    {
        if (currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelaySeconds = hintDelay;
        }
    }
}
