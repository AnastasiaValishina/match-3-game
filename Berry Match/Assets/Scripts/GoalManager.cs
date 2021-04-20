using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    [SerializeField] BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    [SerializeField] GameObject goalPrefab;
    [SerializeField] GameObject goalMenuParent;
    [SerializeField] GameObject goalHudParent;

    EndGameManager endGameManager;
 
    void Start()
    {
        endGameManager = FindObjectOfType<EndGameManager>();
        GetGoals();
        SetGoalsInMenu();  
    }

    void GetGoals()
    {
        if (Board.Instance.level < Board.Instance.world.levels.Length)
        {
            if (Board.Instance != null &&
                Board.Instance.world != null &&
                Board.Instance.world.levels[Board.Instance.level] != null)
            {
                levelGoals = Board.Instance.world.levels[Board.Instance.level].levelGoals;
                for (int i = 0; i < levelGoals.Length; i++)
                {
                    levelGoals[i].numberCollected = 0;
                }
            }
        }
    }

    void SetGoalsInMenu()
    {
        for (int i = 0; i < levelGoals.Length; i++) 
        {
            GameObject goal = Instantiate(goalPrefab, goalMenuParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalMenuParent.transform, false);
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.chipSprite = levelGoals[i].goalSprite;
            panel.chipTag = "0/" + levelGoals[i].numberNeeded;

            GameObject gameGoal = Instantiate(goalPrefab, goalHudParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalHudParent.transform, false);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.chipSprite = levelGoals[i].goalSprite;
            panel.chipTag = "0/" + levelGoals[i].numberNeeded;
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleted = 0;
        for (int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].chipText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].chipText.text = "" + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
        }
        if (goalsCompleted >= levelGoals.Length)
        {
            if(endGameManager != null)
            {
                endGameManager.WinGame();
            }
         }
    }

    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }
}
