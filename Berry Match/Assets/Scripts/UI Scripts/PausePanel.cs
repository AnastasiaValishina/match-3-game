using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PausePanel : MonoBehaviour
{
    void Start()
    {

        GetComponent<RectTransform>().DOAnchorPosY(473, 2f).SetEase(Ease.OutCubic);
    }

    void Update()
    {
        
    }
}
