using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    public float cameraOffset = -10;
    public float aspectRatio = 0.5625f;
    public float padding = 2f;

    Board board;

    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.width, board.height);
            SetOrthoSize(board.width, board.height);
        }
    }

    void RepositionCamera(float width, float height)
    {
        width--;
        height--;
        Vector3 tempPosition = new Vector3(width / 2, height / 2, cameraOffset);
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
