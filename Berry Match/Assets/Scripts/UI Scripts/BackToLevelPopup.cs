using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLevelPopup : MonoBehaviour
{
    public void OnOkClickWin()
    {
        if (GameData.Instance != null)
        {
            GameData.Instance.saveData.isActive[Board.Instance.level + 1] = true;
            GameData.Instance.Save();
        }
        SceneManager.LoadScene(0);
    }

    public void OnOkClickLose()
    {
        SceneManager.LoadScene(0);
    }

}
