using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : MonoBehaviour
{
    public Image[] stars;
    int level;

    int starsActive;

    void OnEnable()
    {
        level = Board.Instance.level;
        LoadData();
        ActivateStars();
    }
    private void LoadData()
    {
        if (GameData.Instance != null)
        {
            starsActive = GameData.Instance.saveData.stars[level];
        }
    }
    void ActivateStars()
    {
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }
}
