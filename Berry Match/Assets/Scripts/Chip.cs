using System.Collections;
using UnityEngine;

public class Chip : MonoBehaviour
{
    [Header("BoardVariables")]
    public int column, row;
    public int previousRow, previousColumn;
    public int targetX, targetY;

    Board board;
    MatchFinder matchFinder;
    public GameObject otherChip;

    Vector2 firstTouchPosition;
    Vector2 finalTouchPosition;
    Vector2 tempPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    public bool isMatched = false;
    public bool isColumnArrow = false;
    public bool isRowArrow = false;
    public bool isColorBomb = false;
    public bool isBomb = false;

    public GameObject rowArrowPrefab;
    public GameObject columnArrowPrefab;
    public GameObject colorBombPrefab;
    public GameObject bombPrefab;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        matchFinder = FindObjectOfType<MatchFinder>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isBomb = true;
            GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            bomb.transform.parent = transform;
        }
    }

    private void Update()
    {
       // FindMatches();
      //  if (isMatched)
       // {
       //     GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 100f);
      //  }

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
            matchFinder.FindAllMatches();
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
            matchFinder.FindAllMatches();
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
        if (isColorBomb)
        {
            matchFinder.MatchChipsOfColor(otherChip.tag);
            isMatched = true;
        }
        else if (otherChip.GetComponent<Chip>().isColorBomb)
        {
            matchFinder.MatchChipsOfColor(gameObject.tag);
            otherChip.GetComponent<Chip>().isMatched = true;
        }
        yield return new WaitForSeconds(0.5f);
        if (otherChip != null)
        {
            if(!isMatched && !otherChip.GetComponent<Chip>().isMatched)
            {
                otherChip.GetComponent<Chip>().row = row;
                otherChip.GetComponent<Chip>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                board.currentChip = null;
                board.currentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
            //otherChip = null;
        }
    }
    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if(board.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    private void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || 
            Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MoveChips();
            board.currentState = GameState.wait;
            board.currentChip = this;
        }
        else
        {
            board.currentState = GameState.move;
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

    public void MakeRowBomb()
    {
        isRowArrow = true;
        GameObject arrow = Instantiate(rowArrowPrefab, transform.position, Quaternion.identity);
        arrow.transform.parent = transform;
    }
    
    public void MakeColumnBomb()
    {
        isColumnArrow = true;
        GameObject arrow = Instantiate(columnArrowPrefab, transform.position, Quaternion.identity);
        arrow.transform.parent = transform;
    }
}
