using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPopup : MonoBehaviour
{
    public Image[] stars;
    public int level;
    [SerializeField] Text highScoreText;
    [SerializeField] Text starCounterText;
    
    int starsActive;
    int highScore;

    void OnEnable()
    {
        LoadData();
        ActivateStars();
        UpdateText();
    }

    private void LoadData()
    {
        if (GameData.Instance != null)
        {
            starsActive = GameData.Instance.saveData.stars[level - 1];
            highScore = GameData.Instance.saveData.highScores[level - 1];
        }
    }

    void UpdateText()
    {
        highScoreText.text = "" + highScore;
        starCounterText.text = "" + starsActive + "/3";
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
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }
}
