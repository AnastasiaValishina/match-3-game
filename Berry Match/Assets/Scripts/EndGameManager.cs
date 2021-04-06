using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    Moves,
    Time
}
[Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}
public class EndGameManager : MonoBehaviour
{
    public EndGameRequirements requirements;
    [SerializeField] Text movesLabel;
    [SerializeField] Text timeLabel;
    [SerializeField] Text counter;
    [SerializeField] GameObject winPopup;
    [SerializeField] GameObject lostPopup;

    int currentCounterValue;
    float timerSeconds;
    Board board;

    void Start()
    {
        board = FindObjectOfType<Board>();
        SetGameType();
        SetupGame();
    }

    private void Update()
    {
       if (requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1f;
            }
        } 
    }

    void SetGameType()
    {
        if (board.level < board.world.levels.Length)
        {
            if (board.world != null)
            {
                if (board.world.levels[board.level] != null)
                {
                    requirements = board.world.levels[board.level].endGameRequirements;
                }
            }
        }
    }

    void SetupGame()
    {
        currentCounterValue = requirements.counterValue;
        if (requirements.gameType == GameType.Moves)
        {
            movesLabel.gameObject.SetActive(true);
            timeLabel.gameObject.SetActive(false);
        }
        else
        {
            timerSeconds = 1f;
            movesLabel.gameObject.SetActive(false);
            timeLabel.gameObject.SetActive(true);
        }
        counter.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;

            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
            counter.text = "" + currentCounterValue;
        }
    }

    public void WinGame()
    {
        winPopup.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        FindObjectOfType<UIAnimationController>().GameOverAnim();
    }

    public void LoseGame()
    {
        lostPopup.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("You lost");
        FindObjectOfType<UIAnimationController>().GameOverAnim();
    }
}
