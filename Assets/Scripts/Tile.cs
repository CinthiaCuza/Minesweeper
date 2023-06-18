using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum State
{
    Unknown,
    Known,
    PossibleMine,
    IsMine,
}

public class Tile : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public Board boardParent;

    [HideInInspector] public bool isMine;
    [HideInInspector] public bool checkComplete;

    public Image backImg;
    public Sprite[] backImgsArray = new Sprite[3];

    [SerializeField] public Image mineImg;
    [SerializeField] public Sprite[] mineImgsArray = new Sprite[2];

    public State state;

    public int minesAround;
    public TMP_Text textMinesAround;
    private Color numberColor = new Color();

    public Vector2Int pos;
    public List<Vector2Int> posAroundList = new List<Vector2Int>();

    private void Start()
    {
        GameController.GameOverEvent.AddListener(ShowTileGO);
        GameController.RestartEvent.AddListener(DestroyTile);
    }

    public void CalculatePosAround()
    {
        int rows = boardParent.maxrows;
        int columns = boardParent.maxcolumns;

        if (pos.x - 1 >= 0 && pos.x - 1 < rows && pos.y - 1 >= 0 && pos.y - 1 < columns)
            posAroundList.Add(new Vector2Int(pos.x - 1, pos.y - 1));

        if (pos.x - 1 >= 0 && pos.x - 1 < rows && pos.y >= 0 && pos.y < columns)
            posAroundList.Add(new Vector2Int(pos.x - 1, pos.y));

        if (pos.x - 1 >= 0 && pos.x - 1 < rows && pos.y + 1 >= 0 && pos.y + 1 < columns)
            posAroundList.Add(new Vector2Int(pos.x - 1, pos.y + 1));

        if(pos.x >= 0 && pos.x < rows && pos.y - 1 >= 0 && pos.y - 1 < columns)
            posAroundList.Add(new Vector2Int(pos.x, pos.y - 1));

        if(pos.x >= 0 && pos.x < rows && pos.y + 1 >= 0 && pos.y + 1 < columns)
            posAroundList.Add(new Vector2Int(pos.x, pos.y + 1));

        if(pos.x + 1 >= 0 && pos.x + 1 < rows && pos.y - 1 >= 0 && pos.y - 1 < columns)
            posAroundList.Add(new Vector2Int(pos.x + 1, pos.y - 1));

        if (pos.x + 1 >= 0 && pos.x + 1 < rows && pos.y >= 0 && pos.y < columns)
            posAroundList.Add(new Vector2Int(pos.x + 1, pos.y));

        if (pos.x + 1 >= 0 && pos.x + 1 < rows && pos.y + 1 >= 0 && pos.y + 1 < columns)
            posAroundList.Add(new Vector2Int(pos.x + 1, pos.y + 1));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!boardParent.isGameOver && !boardParent.botPlaying)
        {
            if (eventData.button == PointerEventData.InputButton.Left) LeftClick();
            if (eventData.button == PointerEventData.InputButton.Right) RightClick();
        }
    }

    public void LeftClick()
    {
        if (state.Equals(State.Unknown))
        {
            backImg.enabled = false;
            state = State.Known;

            if (isMine)
            {
                mineImg.sprite = mineImgsArray[0];
                mineImg.gameObject.SetActive(true);

                boardParent.textRemainingMines.text = "00";
                boardParent.restartEmojiImg.sprite = boardParent.gameController.emojisArray[1];
                boardParent.isGameOver = true;

                GameController.GameOverEvent.Invoke();
            }
            else
            {
                RevealTile();
            }
        }
    }
    private void RevealTile()
    {
        for (int i = 0; i < posAroundList.Count; i++)
        {
            if (boardParent.boardDataBase[posAroundList[i].x, posAroundList[i].y].isMine) minesAround++;
        }

        if (minesAround != 0) NumberColor();
        else ExpandMap();
    }

    private void NumberColor()
    {
        switch (minesAround)
        {
            case 1:
                ColorUtility.TryParseHtmlString("#0B24FB", out numberColor);
                break;
            case 2:
                ColorUtility.TryParseHtmlString("#0E7A11", out numberColor);
                break;
            case 3:
                ColorUtility.TryParseHtmlString("#FC0D1B", out numberColor);
                break;
            case 4:
                ColorUtility.TryParseHtmlString("#020B79", out numberColor);
                break;
            case 5:
                ColorUtility.TryParseHtmlString("#852123", out numberColor);
                break;
            case 6:
                ColorUtility.TryParseHtmlString("#278786", out numberColor);
                break;
            case 7:
                ColorUtility.TryParseHtmlString("#000000", out numberColor);
                break;
            case 8:
                ColorUtility.TryParseHtmlString("#878787", out numberColor);
                break;
        }

        textMinesAround.color = numberColor;
        textMinesAround.text = minesAround.ToString();

        boardParent.tileWithNumberList.Add(this);
    }

    private void ExpandMap()
    {
        for (int i = 0; i < posAroundList.Count; i++)
        {
            Tile tileAround = boardParent.boardDataBase[posAroundList[i].x, posAroundList[i].y];
            if (!tileAround.isMine) tileAround.LeftClick();
        }
    }

    private void RightClick()
    {
        if (state.Equals(State.Unknown))
        {
            state = State.PossibleMine;
            backImg.sprite = backImgsArray[1];
            boardParent.possibleMinesList.Add(this);
        }
        else if (state.Equals(State.PossibleMine))
        {
            state = State.Unknown;
            backImg.sprite = backImgsArray[0];
            boardParent.possibleMinesList.Remove(this);
        }
        else if (state.Equals(State.IsMine))
        {
            state = State.Unknown;
            backImg.sprite = backImgsArray[0];
            boardParent.isMinesList.Remove(this);
        }

        if(!state.Equals(State.Known)) boardParent.UpdateCounter();
    }

    private void ShowTileGO()
    {
        if (state.Equals(State.Unknown) && boardParent.isMinesList.Count != boardParent.maxMines && isMine)
        {
            backImg.enabled = false;

            mineImg.sprite = mineImgsArray[1];
            mineImg.gameObject.SetActive(true);
        }

        if (state.Equals(State.Unknown) && boardParent.isMinesList.Count == boardParent.maxMines) LeftClick();
    }

    private void DestroyTile()
    {
        Destroy(gameObject);
    }

    #region Bot

    public IEnumerator FindMinesAround()
    {
        List<Vector2Int> possibleMinesAroundList = new List<Vector2Int>();
        int mines = 0;

        for (int i = 0; i < posAroundList.Count; i++)
        {
            Tile tileAround = boardParent.boardDataBase[posAroundList[i].x, posAroundList[i].y];

            if (tileAround.state.Equals(State.Unknown)) possibleMinesAroundList.Add(tileAround.pos);
            else if (tileAround.state.Equals(State.IsMine)) mines++;
        }

        if (mines == minesAround)
        {
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < possibleMinesAroundList.Count; i++)
            {
                Tile mine = boardParent.boardDataBase[possibleMinesAroundList[i].x, possibleMinesAroundList[i].y];

                mine.backImg.sprite = mine.backImgsArray[2];
                yield return new WaitForSeconds(1f);

                mine.LeftClick();
                boardParent.boardChange = true;
            }

            checkComplete = true;
        }
        else
        {
            if (possibleMinesAroundList.Count + mines == minesAround)
            {
                for (int i = 0; i < possibleMinesAroundList.Count; i++)
                {
                    Tile mine = boardParent.boardDataBase[possibleMinesAroundList[i].x, possibleMinesAroundList[i].y];

                    mine.state = State.IsMine;
                    mine.backImg.sprite = mine.backImgsArray[1];

                    boardParent.isMinesList.Add(this);
                    boardParent.UpdateCounter();

                    if (boardParent.isGameOver) break;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
    

    #endregion
}
