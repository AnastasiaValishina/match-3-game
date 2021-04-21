using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Chip : MonoBehaviour
{
    public int column, row;
    public int previousRow, previousColumn;
    public int targetX, targetY;

    [SerializeField] Sprite colorBombSprite;
    [SerializeField] Sprite rowBombSprite;
    [SerializeField] Sprite columnBombSprite;
    [SerializeField] Sprite bombSprite;

    public bool isMatched = false;
    public bool isColumnArrow = false;
    public bool isRowArrow = false;
    public bool isColorBomb = false;
    public bool isBomb = false;
    public Chip otherChip;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    Vector2 firstTouchPosition = Vector2.zero;
    Vector2 finalTouchPosition = Vector2.zero;
    Vector2 tempPosition;
    SpriteRenderer chipSprite;

    private void Start()
    {
        chipSprite = GetComponent<SpriteRenderer>();
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
            if (Board.Instance.allChips[column, row] != gameObject)
            {
                Board.Instance.allChips[column, row] = this;
            //    matchFinder.FindAllMatches();
            }
            MatchFinder.Instance.FindAllMatches();
        }
        else
        {
            //diractly set position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            Board.Instance.allChips[column, row] = this;
        }
        if (Mathf.Abs(targetY - transform.position.y) > 0.1)
        {
            //Move towards target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.6f);
            if (Board.Instance.allChips[column, row] != gameObject)
            {
                Board.Instance.allChips[column, row] = this;
           //     matchFinder.FindAllMatches();
            }
            MatchFinder.Instance.FindAllMatches();
        }
        else
        {
            //directly set position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            Board.Instance.allChips[column, row] = this;
        }
    }

    public IEnumerator CheckMove()
    {
        if (isColorBomb)
        {
            MatchFinder.Instance.MatchChipsOfColor(otherChip.tag);
            isMatched = true;
        }
        else if (otherChip.isColorBomb)
        {
            MatchFinder.Instance.MatchChipsOfColor(gameObject.tag);
            otherChip.isMatched = true;
        }
        yield return new WaitForSeconds(0.5f);
        if (otherChip != null)
        {
            if(!isMatched && !otherChip.isMatched)
            {
                otherChip.row = row;
                otherChip.column = column;
                otherChip.column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                Board.Instance.currentChip = null;
                Board.Instance.currentState = GameState.move;
            }
            else
            {
                if (EndGameManager.Instance != null)
                {
                    if (EndGameManager.Instance.requirements.gameType == GameType.Moves)
                    {
                        EndGameManager.Instance.DecreaseCounterValue();
                    }
                }
                Board.Instance.DestroyMatches();
            }
            //otherChip = null;
        }
    }
    private void OnMouseDown()
    {
        if (HintManager.Instance != null)
        {
            HintManager.Instance.DestroyHint();
        }

        AnimationTouch();

        if (Board.Instance.currentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {
        if(Board.Instance.currentState == GameState.move)
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
            Board.Instance.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            Swipe();
            Board.Instance.currentChip = this;
        }
        else
        {
            Board.Instance.currentState = GameState.move;
        }
    }

    void MoveChips(Vector2 direction)
    {
        otherChip = Board.Instance.allChips[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        if (Board.Instance.lockTiles[column, row] == null && Board.Instance.lockTiles[column + (int)direction.x, row + (int)direction.y] == null)
        {
            if (otherChip != null)
            {
                otherChip.column += -1 * (int)direction.x;
                otherChip.row += -1 * (int)direction.y;
                column += (int)direction.x;
                row += (int)direction.y;
                StartCoroutine(CheckMove());
            }
            else
            {
                Board.Instance.currentState = GameState.move;
            }
        }
        else
        {
            Board.Instance.currentState = GameState.move;
        }
    }

    private void Swipe()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < Board.Instance.width - 1)
        {
            MoveChips(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < Board.Instance.height - 1)
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
            Board.Instance.currentState = GameState.move;
        }
    }

    public void MakeRowBomb()
    {
        if (!isColumnArrow && !isColorBomb && !isBomb)
        {
            isRowArrow = true;
            chipSprite.sprite = rowBombSprite;
        }
    }
    
    public void MakeColumnBomb()
    {
        if (!isRowArrow && !isColorBomb && !isBomb)
        {
            isColumnArrow = true;
            chipSprite.sprite = columnBombSprite;
        }
    }

    public void MakeColorBomb()
    {
        if (!isRowArrow &&!isColumnArrow && !isBomb)
        {
            isColorBomb = true;
            chipSprite.sprite = colorBombSprite;
            gameObject.tag = "RainbowBomb";
        }
    }

    public void MakeAdjacentBomb()
    {
        if (!isRowArrow && !isColumnArrow && !isColorBomb)
        {
            isBomb = true;
            chipSprite.sprite = bombSprite;
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
