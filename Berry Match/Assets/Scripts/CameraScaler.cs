using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    [SerializeField] float cameraOffset = -10;
    [SerializeField] float aspectRatio = 0.5625f;
    [SerializeField] float padding = 2f;
    [SerializeField] float yOffset = 1f;

    void Start()
    {
        RepositionCamera(Board.Instance.width, Board.Instance.height);
        SetOrthoSize(Board.Instance.width, Board.Instance.height);
    }

    void RepositionCamera(float width, float height)
    {
        width--;
        height--;
        Vector3 tempPosition = new Vector3(width / 2, height / 2 + yOffset, cameraOffset);
        transform.position = tempPosition;
    }

    private void SetOrthoSize(float width, float height)
    {
        if (width >= height)
        {
            Camera.main.orthographicSize = (width / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = height / 2 + padding;
        }
    }
}
