using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelItem : MonoBehaviour
{
    public Image[] stars;
    public Text levelText;
    public int level;
    public GameObject confirmPopup;
    public bool isActive;
    public Image shadow;

    Button itemButton;
    GameData gameData;
    int starsActive;

    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        itemButton = GetComponent<Button>();
        LoadData();
        ActivateStars();
        ShowLevel();
        SetSprite();
    }

    private void LoadData()
    {
        if (gameData != null)
        {
            if (gameData.saveData.isActive[level - 1])
            {
                isActive = true;
            }
            else
            {
                isActive = false;
            }

            starsActive = gameData.saveData.stars[level - 1];
        }
    }

    void SetSprite()
    {
        if (isActive)
        {
            shadow.gameObject.SetActive(false);
            itemButton.enabled = true;
        }
        else
        {
            shadow.gameObject.SetActive(true);
            itemButton.enabled = false;
        }
    }

    public void ShowConfirmPopup(int level)
    {
        confirmPopup.GetComponent<ConfirmPopup>().level = level;
        confirmPopup.SetActive(true);
    }

    void ShowLevel()
    {
        levelText.text = "" + level;
    }

    void ActivateStars()
    {
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }
}
