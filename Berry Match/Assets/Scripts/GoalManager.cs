using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BlankGoal
{
    public int numberNeeded;
    [SerializeField] int numberCollected;
    public Sprite goalSprite;
    public string matchValue;
}

public class GoalManager : MonoBehaviour
{
    [SerializeField] BlankGoal[] levelGoals;
    [SerializeField] GameObject goalPrefab;
    [SerializeField] GameObject goalMenuParent;
    [SerializeField] GameObject goalHudParent;

    void Start()
    {
        SetGoalsInMenu();  
    }


    void Update()
    {
        
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
            panel.chipSprite = levelGoals[i].goalSprite;
            panel.chipTag = "0/" + levelGoals[i].numberNeeded;
        }
    }
}
