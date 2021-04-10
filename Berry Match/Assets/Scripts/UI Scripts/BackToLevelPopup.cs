using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLevelPopup : MonoBehaviour
{
    GameData gameData;
    Board board;

    private void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<Board>();
    }
    public void OnOkClickWin()
    {
        if (gameData != null)
        {
            gameData.saveData.isActive[board.level + 1] = true;
            gameData.Save();
        }
        SceneManager.LoadScene(0);
    }

    public void OnOkClickLose()
    {
        SceneManager.LoadScene(0);
    }

}
