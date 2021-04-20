using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Text scoreText;
    [SerializeField] Image scoreBar;
    public int score;
    GameData gameData;
    int numberStars;

    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        scoreText.text = score.ToString();
    }

    void Update()
    {

    }

    public void IncreaseScore(int amount)
    {
        score += amount;

        for (int i = 0; i < Board.Instance.scoreGoals.Length; i++)
        {
            if (score > Board.Instance.scoreGoals[i] && numberStars < i + 1)
            {
                numberStars++;
            }
        }

        if (gameData != null)
        {
            int highScore = gameData.saveData.highScores[Board.Instance.level];
            if (score > highScore)
            {
                gameData.saveData.highScores[Board.Instance.level] = score;
                gameData.saveData.stars[Board.Instance.level] = numberStars;
            }

            int currentStars = gameData.saveData.stars[Board.Instance.level];
            if (numberStars > currentStars)
            {
                gameData.saveData.stars[Board.Instance.level] = numberStars;
            }

            gameData.Save();
        }

        scoreText.text = score.ToString();
        
        if (Board.Instance != null && scoreBar != null)
        {
            int length = Board.Instance.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)Board.Instance.scoreGoals[length - 1];
        }
    }
}
