using System;
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

    public int currentCounterValue;
    float timerSeconds;
    static EndGameManager instance;
    public static EndGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EndGameManager>();
            }
            return instance;
        }
    }

    void Start()
    {
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
        if (Board.Instance.level < Board.Instance.world.levels.Length)
        {
            if (Board.Instance.world != null)
            {
                if (Board.Instance.world.levels[Board.Instance.level] != null)
                {
                    requirements = Board.Instance.world.levels[Board.Instance.level].endGameRequirements;
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
        if (Board.Instance.currentState != GameState.pause)
        {
            currentCounterValue--;

            if (currentCounterValue <= 0)
            {
                UIAnimationController.Instance.LoseGame();
            }
            counter.text = "" + currentCounterValue;
        }
    }
}
