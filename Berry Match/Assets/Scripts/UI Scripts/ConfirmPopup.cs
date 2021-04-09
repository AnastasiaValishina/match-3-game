using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPopup : MonoBehaviour
{
    public Image[] stars;
    public int level;
    void Start()
    {
        ActivateStars();
    }

    void Update()
    {
        
    }

    public void OnCloseClick()
    {
        gameObject.SetActive(false);
    }

    public void OnPlayClicked()
    {
        PlayerPrefs.SetInt("Current Level", level - 1);
        SceneManager.LoadScene(1);
    }

    void ActivateStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = false;
        }
    }
}
