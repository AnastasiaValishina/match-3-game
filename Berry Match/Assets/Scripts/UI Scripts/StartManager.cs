using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject levelScreen;

    void Start()
    {
    //    startScreen.SetActive(true);
        levelScreen.SetActive(false);
    }

    void Update()
    {
        
    }

    public void PlayGame()
    {
        startScreen.SetActive(false);
        levelScreen.SetActive(true);
    }

    public void Home()
    {
        startScreen.SetActive(true);
        levelScreen.SetActive(false);
    }
}
