using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIAnimationController : MonoBehaviour
{
    [SerializeField] Image fadePanel;
    [SerializeField] RectTransform goalsPopup;
    void Start()
    {
        goalsPopup.DOAnchorPosY(0f, 1.5f);
    }

    public void OnOkClicked()
    {
        goalsPopup.DOAnchorPosY(1600f, 1.5f);
        fadePanel.DOFade(0f, 1.5f);
        StartCoroutine(GameStart());
    } 

    public void GameOverAnim()
    {
        fadePanel.DOFade(1f, 1.5f);
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1.5f);
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;
    }
}
