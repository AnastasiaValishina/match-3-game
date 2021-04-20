using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Chip : MonoBehaviour
{
    public int column, row;
    public int previousRow, previousColumn;
    public int targetX, targetY;

    [SerializeField] GameObject rowArrowPrefab;
    [SerializeField] GameObject columnArrowPrefab;
    [SerializeField] GameObject bombPrefab;
    [SerializeField] Sprite colorBomb;
    [SerializeField] Sprite bombSprite;

    public bool isMatched = false;
    public bool isColumnArrow = false;
    public bool isRowArrow = false;
    public bool isColorBomb = false;
    public bool isBomb = false;
    public GameObject otherChip;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    Board board;
    MatchFinder matchFinder;
    HintManager hintManager;
    EndGameManager endGameManager;

    Vector2 firstTouchPosition = Vector2.zero;
    Vector2 finalTouchPosition = Vector2.zero;
    Vector2 tempPosition;
    SpriteRenderer myImage;

    private void Start()
    {
        board = FindObjectOfType<Board>();
        matchFinder = FindObjectOfType<MatchFinder>();
        hintManager = FindObjectOfType<HintManager>();
        endGameManager = FindObjectOfType<EndGameManager>();
        myImage = GetComponent<SpriteRenderer>();
        AnimationLanding();
    }

    private void Update()
    {
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
            //    matchFinder.FindAllMatches();
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
           //     matchFinder.FindAllMatches();
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
                if (endGameManager != null)
                {
                    if (endGameManager.requirements.gameType == GameType.Moves)
                    {
                        endGameManager.DecreaseCounterValue();
                    }
                }
                board.DestroyMatches();
            }
            //otherChip = null;
        }
    }
    private void OnMouseDown()
    {
        if (hintManager != null)
        {
            hintManager.DestroyHint();
        }

        AnimationTouch();

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
            board.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            Swipe();
            board.currentChip = this;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MoveChips(Vector2 direction)
    {
        otherChip = board.allChips[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        if (board.lockTiles[column, row] == null && board.lockTiles[column + (int)direction.x, row + (int)direction.y] == null)
        {
            if (otherChip != null)
            {
                otherChip.GetComponent<Chip>().column += -1 * (int)direction.x;
                otherChip.GetComponent<Chip>().row += -1 * (int)direction.y;
                column += (int)direction.x;
                row += (int)direction.y;
                StartCoroutine(CheckMove());
            }
            else
            {
                board.currentState = GameState.move;
            }
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    private void Swipe()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            MoveChips(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            MoveChips(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            MoveChips(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            MoveChips(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    public void MakeRowBomb()
    {
        if (!isColumnArrow && !isColorBomb && !isBomb)
        {
            isRowArrow = true;
            GameObject arrow = Instantiate(rowArrowPrefab, transform.position, Quaternion.identity);
            arrow.transform.parent = transform;
        }
    }
    
    public void MakeColumnBomb()
    {
        if (!isRowArrow && !isColorBomb && !isBomb)
        {
            isColumnArrow = true;
            GameObject arrow = Instantiate(columnArrowPrefab, transform.position, Quaternion.identity);
            arrow.transform.parent = transform;
        }
    }

    public void MakeColorBomb()
    {
        if (!isRowArrow &&!isColumnArrow && !isBomb)
        {
            isColorBomb = true;
            myImage.sprite = colorBomb;
            gameObject.tag = "RainbowBomb";
        }
    }

    public void MakeAdjacentBomb()
    {
        if (!isRowArrow && !isColumnArrow && !isColorBomb)
        {
            isBomb = true;
            GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            bomb.transform.parent = transform;
        }
    }

    void AnimationTouch()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOScaleX(0.8f, 0.3f));
        mySequence.Append(transform.DOScaleX(1f, 0.3f));
        mySequence.Join(transform.DOScaleY(0.8f, 0.3f));
        mySequence.Append(transform.DOScaleY(1f, 0.3f));
    }

    void AnimationLanding()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendInterval(0.2f);
        mySequence.Append(transform.DOScaleY(0.7f, 0.2f));
        mySequence.Append(transform.DOScaleY(1.1f, 0.2f));
        mySequence.Append(transform.DOScaleY(1f, 0.2f));
    }
}
