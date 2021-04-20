using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject shadowPanel;
    [SerializeField] Image soundButtonIcon;
    [SerializeField] Sprite musicOnSprite;
    [SerializeField] Sprite musicOffSprite;

    public bool isPaused = false;
    SoundManager soundManager;
    
    void Start()
    {
        shadowPanel.SetActive(false);
        soundManager = FindObjectOfType<SoundManager>();

        // if sound = 0 mute, if sound = 1 unmute
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButtonIcon.sprite = musicOffSprite;
            }
            else
            {
                soundButtonIcon.sprite = musicOnSprite;
            }
        }
        else
        {
            soundButtonIcon.sprite = musicOnSprite;
        }
    }

    public void SwitchSound()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                soundButtonIcon.sprite = musicOnSprite;
                PlayerPrefs.SetInt("Sound", 1);
                soundManager.AdjustVolume();
            }
            else
            {
                soundButtonIcon.sprite = musicOffSprite;
                PlayerPrefs.SetInt("Sound", 0);
                soundManager.AdjustVolume();
            }
        }
        else
        {
            soundButtonIcon.sprite = musicOffSprite;
            PlayerPrefs.SetInt("Sound", 1);
            soundManager.AdjustVolume();
        }
    }
    public void PauseGame()
    {
        if (isPaused)
        {
            shadowPanel.SetActive(false);
            pausePanel.GetComponent<RectTransform>().DOAnchorPosY(-522, 1f).SetEase(Ease.InSine);
            Board.Instance.currentState = GameState.move;
        }

        if (!isPaused)
        {
            shadowPanel.SetActive(true);
            pausePanel.GetComponent<RectTransform>().DOAnchorPosY(473, 1f).SetEase(Ease.OutSine);
            Board.Instance.currentState = GameState.pause;
        }
        isPaused = !isPaused;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }
}
