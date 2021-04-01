using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalPanel : MonoBehaviour
{
    [SerializeField] Image icon;
    public Sprite chipSprite;
    [SerializeField] Text text;
    public string chipTag;

    void Start()
    {
        Setup();
    }

    void Update()
    {
        
    }

    void Setup()
    {
        icon.sprite = chipSprite;
        text.text = chipTag;
    }
}
