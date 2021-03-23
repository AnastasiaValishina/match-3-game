using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chip : MonoBehaviour
{
    [Header("BoardVariables")]
    public int column, row;
    public int previousRow, previousColumn;
    public int targetX, targetY;

    Board board;
    GameObject otherChip;
    Vector2 firstTouchPosition;
    Vector2 finalTouchPosition;
    Vector2 tempPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f;
    public bool isMatched = false;

    private void Start()
    {
        board = FindObjectOfType<Board>();
       // targetX = (int)transform.position.x;
       // targetY = (int)transform.position.y;
       // row = targetY;
       // column = targetX;
    }

    private void Update()
    {
        FindMatches();
        if (isMatched)
        {
            GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 100f);
        }

        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > 0.1)
        {
            //Move towards target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.6f);
            if (board.allChips[column, row] != gameObject)
            {
                board.allChips[column, row] = gameObject;
            }
        }
        else
        {
            //diractly set position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allChips[column, row] = gameObject;
        }
        if (Mathf.Abs(targetY - transform.position.y) > 0.1)
        {
            //Move towards target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.6f);
            if (board.allChips[column, row] != gameObject)
            {
                board.allChips[column, row] = gameObject;
            }
        }
        else
        {
            //directly set position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allChips[column, row] = gameObject;
        }
    }

    public IEnumerator CheckMove()
    {
        yield return new WaitForSeconds(0.5f);
        if (otherChip != null)
        {
            if(!isMatched && !otherChip.GetComponent<Chip>().isMatched)
            {
                otherChip.GetComponent<Chip>().row = row;
                otherChip.GetComponent<Chip>().column = column;
                row = previousRow;
                column = previousColumn;
            }
            else
            {
                board.DestroyMatches();
            }
            otherChip = null;
        }
    }
    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    private void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || 
            Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MoveChips();
        }
    }

    private void MoveChips()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            // right swipe
            otherChip = board.allChips[column + 1, row];
            previousRow = row;
            previousColumn = column;
            otherChip.GetComponent<Chip>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            // up swipe
            otherChip = board.allChips[column, row + 1];
            previousRow = row;
            previousColumn = column;
            otherChip.GetComponent<Chip>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            // left swipe
            otherChip = board.allChips[column - 1, row];
            previousRow = row;
            previousColumn = column;
            otherChip.GetComponent<Chip>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < 45 && swipeAngle >= -135 && row > 0)
        {
            // down swipe
            otherChip = board.allChips[column, row - 1];
            previousRow = row;
            previousColumn = column;
            otherChip.GetComponent<Chip>().row += 1;
            row -= 1;
        }

        StartCoroutine(CheckMove());
    }

    private void FindMatches()
    {
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftChip1 = board.allChips[column - 1, row];
            GameObject rightChip1 = board.allChips[column + 1, row];
            if (leftChip1 != null && rightChip1 != null)
            {
                if (leftChip1.tag == gameObject.tag && rightChip1.tag == gameObject.tag)
                {
                    isMatched = true;
                    leftChip1.GetComponent<Chip>().isMatched = true;
                    rightChip1.GetComponent<Chip>().isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upperChip1 = board.allChips[column, row + 1];
            GameObject lowerChip1 = board.allChips[column, row - 1];
            if (upperChip1 != null && lowerChip1 != null)
            {
                if (upperChip1.tag == gameObject.tag && lowerChip1.tag == gameObject.tag)
                {
                    isMatched = true;
                    upperChip1.GetComponent<Chip>().isMatched = true;
                    lowerChip1.GetComponent<Chip>().isMatched = true;
                }
            }
        }
    }
}
