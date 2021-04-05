using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableTile : MonoBehaviour
{
    public int hitPoints;
    SpriteRenderer spriteRenderer;
    GoalManager goalManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        goalManager = FindObjectOfType<GoalManager>();
    }

    private void Update() 
    {
        if (hitPoints <= 0)
        {
            if (goalManager != null)
            {
                goalManager.CompareGoal(gameObject.tag);
                goalManager.UpdateGoals();
            }
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();
    }

    void MakeLighter()
    {
        Color color = spriteRenderer.color;
        float newAlpha = color.a / 2;
        spriteRenderer.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}
