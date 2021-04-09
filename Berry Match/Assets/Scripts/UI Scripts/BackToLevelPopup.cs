using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLevelPopup : MonoBehaviour
{
    public void OnOkClick()
    {
        SceneManager.LoadScene(0);
    }

}
