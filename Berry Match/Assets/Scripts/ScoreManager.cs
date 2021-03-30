using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] Image scoreBar;
    public int score;
    Board board;

    void Start()
    {
        scoreText.text = score.ToString();
        board = FindObjectOfType<Board>();
    }

    void Update()
    {

    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
        
        if (board != null && scoreBar != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals[length - 1];
        }
    }
}
