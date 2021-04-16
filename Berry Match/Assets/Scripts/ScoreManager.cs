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
    int numberStars;

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

        for (int i = 0; i < board.scoreGoals.Length; i++)
        {
            if (score > board.scoreGoals[i] && numberStars < i + 1)
            {
                numberStars++;
            }
        }

        if (gameData != null)
        {
            int highScore = gameData.saveData.highScores[board.level];
            if (score > highScore)
            {
                gameData.saveData.highScores[board.level] = score;
                gameData.saveData.stars[board.level] = numberStars;
            }

            int currentStars = gameData.saveData.stars[board.level];
            if (numberStars > currentStars)
            {
                gameData.saveData.stars[board.level] = numberStars;
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
