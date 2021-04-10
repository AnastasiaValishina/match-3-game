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
    GameData gameData;

    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
        scoreText.text = score.ToString();
    }

    void Update()
    {

    }

    public void IncreaseScore(int amount)
    {
        score += amount;

        if (gameData != null)
        {
            int highScore = gameData.saveData.highScores[board.level];
            if (score > highScore)
            {
                gameData.saveData.highScores[board.level] = score;
            }
            gameData.Save();
        }

        scoreText.text = score.ToString();
        
        if (board != null && scoreBar != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals[length - 1];
        }
    }
}
