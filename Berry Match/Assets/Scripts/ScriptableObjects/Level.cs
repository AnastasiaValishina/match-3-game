using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName ="Level")]
public class Level : ScriptableObject
{
    [Header("Board")]
    public int width = 8;
    public int height = 8;
    public TileType[] boardLayout;
    public Chip[] chips;
    public int[] scoreGoals;
    public EndGameRequirements endGameRequirements;
    public BlankGoal[] levelGoals;
}
