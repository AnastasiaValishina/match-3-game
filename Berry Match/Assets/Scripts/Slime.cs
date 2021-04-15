using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Slime : MonoBehaviour
{
    void Start()
    {
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        //Sequence mySequence = DOTween.Sequence();
    //    mySequence.Append(
            transform.DOScale(new Vector3(1f, 1f, 1f), 1f).SetEase(Ease.OutElastic);
    }

    void Update()
    {
        
    }
}
