using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIAnimationController : MonoBehaviour
{
    [SerializeField] Image fadePanel;
    [SerializeField] RectTransform goalsPopup;
    [SerializeField] RectTransform winPopup;
    [SerializeField] RectTransform lostPopup;

    static UIAnimationController instance;
    public static UIAnimationController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIAnimationController>();
            }
            return instance;
        }
    }
    void Start()
    {
        Sequence goalPopupSequence = DOTween.Sequence();
        goalPopupSequence.Append(goalsPopup.DOAnchorPosY(0f, 1.5f).SetEase(Ease.OutSine));
        goalPopupSequence.AppendInterval(1f);
        goalPopupSequence.Append(goalsPopup.DOAnchorPosY(-1800f, 1.5f).SetEase(Ease.InSine));
        goalPopupSequence.Join(fadePanel.DOFade(0f, 1.5f));

        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1.5f);
        Board.Instance.currentState = GameState.move;
    }

    public void WinGame()
    {
        winPopup.gameObject.SetActive(true);
        fadePanel.DOFade(1f, 1.5f);
        winPopup.DOAnchorPosY(0f, 1.5f).SetEase(Ease.OutSine);
        Board.Instance.currentState = GameState.win;
        EndGameManager.Instance.currentCounterValue = 0;
    }

    public void LoseGame()
    {
        winPopup.gameObject.SetActive(true);
        fadePanel.DOFade(1f, 1.5f);
        lostPopup.DOAnchorPosY(0f, 1.5f).SetEase(Ease.OutSine);
        Board.Instance.currentState = GameState.lose;
    }
}
