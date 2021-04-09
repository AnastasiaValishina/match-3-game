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


 //   Image buttonImage;
    Button itemButton;

    void Start()
    {
    //    buttonImage = GetComponent<Image>();
        itemButton = GetComponent<Button>();
        ActivateStars();
        ShowLevel();
        SetSprite();
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
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = false;
        }
    }
}
