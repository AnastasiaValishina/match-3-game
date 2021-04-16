using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] Image soundButtonIcon;
    [SerializeField] Sprite musicOnSprite;
    [SerializeField] Sprite musicOffSprite;

    public bool isPaused = false;
    Board board;
    SoundManager soundManager;

    
    void Start()
    {
        board = FindObjectOfType<Board>();
        pausePanel.SetActive(false);
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

    private void Update()
    {
        if (isPaused && !pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            board.currentState = GameState.pause;
        }
        if (!isPaused && pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            board.currentState = GameState.move;
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
        pausePanel.GetComponent<RectTransform>().DOAnchorPosY(473, 2f).SetEase(Ease.OutCubic);
        if (isPaused)
        {
            pausePanel.GetComponent<RectTransform>().DOAnchorPosY(-522, 2f).SetEase(Ease.InCubic);
        }
        isPaused = !isPaused;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }
}
