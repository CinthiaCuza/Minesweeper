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
    [HideInInspector] 
    public Board boardParent;

    public bool isMine;
    public bool checkComplete;

    public Image backImg;
    public Image mineImg;

    public Sprite unknownTileImg;
    public Sprite flagImg;
    public Sprite selectedTileImg;
    public Sprite redMineImg;
    public Sprite showMineImg;

    public State state;

    public int minesAround;
    public TMP_Text textMinesAround;
    private Color numberColor = new Color();
    private Dictionary<int, string> colors = new Dictionary<int, string>();

    public Vector2Int pos;
    public List<Vector2Int> posAroundList = new List<Vector2Int>();

    private void Start()
    {
        GameController.GameOverEvent.AddListener(ShowTileGO);
        GameController.RestartEvent.AddListener(DestroyTile);

        colors.Add(1, "#0B24FB");
        colors.Add(2, "#0E7A11");
        colors.Add(3, "#FC0D1B");
        colors.Add(4, "#020B79");
        colors.Add(5, "#852123");
        colors.Add(6, "#278786");
        colors.Add(7, "#000000");
        colors.Add(8, "#878787");
    }

    public void CalculatePosAround()
    {
        int rows = boardParent.boardConf.maxrows;
        int columns = boardParent.boardConf.maxcolumns;

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
                mineImg.sprite = redMineImg;
                mineImg.gameObject.SetActive(true);

                boardParent.textRemainingMines.text = "00";
                boardParent.restartEmojiImg.sprite = boardParent.gameController.emojiSad;
                boardParent.isGameOver = true;

                GameController.GameOverEvent.Invoke();
            }
            else RevealTile();
        }
    }
    private void RevealTile()
    {
        foreach (Vector2Int vectorPos in posAroundList)
        {
            if (boardParent.boardDataBase[vectorPos.x, vectorPos.y].isMine) minesAround++;
        }

        if (minesAround != 0) NumberColor();
        else ExpandMap();
    }

    private void NumberColor()
    {
        ColorUtility.TryParseHtmlString(colors[minesAround], out numberColor);

        textMinesAround.color = numberColor;
        textMinesAround.text = minesAround.ToString();

        boardParent.tileWithNumberList.Add(this);
    }

    private void ExpandMap()
    {
        foreach (Vector2Int vectorPos in posAroundList)
        {
            Tile tileAround = boardParent.boardDataBase[vectorPos.x, vectorPos.y];
            if (!tileAround.isMine) tileAround.LeftClick();
        }
    }

    private void RightClick()
    {
        switch (state)
        {
            case State.Unknown:
                state = State.PossibleMine;
                backImg.sprite = flagImg;
                boardParent.possibleMinesList.Add(this);
                boardParent.UpdateCounter();
                break;
            case State.PossibleMine:
                state = State.Unknown;
                backImg.sprite = unknownTileImg;
                boardParent.possibleMinesList.Remove(this);
                boardParent.UpdateCounter();
                break;
            case State.IsMine:
                state = State.Unknown;
                backImg.sprite = unknownTileImg;
                boardParent.isMinesList.Remove(this);
                boardParent.UpdateCounter();
                break;
        }
    }

    private void ShowTileGO()
    {
        if (state.Equals(State.Unknown) && boardParent.isMinesList.Count != boardParent.boardConf.maxMines && isMine)
        {
            backImg.enabled = false;
            mineImg.sprite = showMineImg;
            mineImg.gameObject.SetActive(true);
        }
        else if (state.Equals(State.Unknown) && boardParent.isMinesList.Count == boardParent.boardConf.maxMines) LeftClick();
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

        foreach (Vector2Int vectorPos in posAroundList)
        {
            Tile tileAround = boardParent.boardDataBase[vectorPos.x, vectorPos.y];

            if (tileAround.state.Equals(State.Unknown)) possibleMinesAroundList.Add(tileAround.pos);
            else if (tileAround.state.Equals(State.IsMine)) mines++;
        }

        if (mines == minesAround)
        {
            yield return new WaitForSeconds(boardParent.gameController.botSpeedValue);

            foreach (Vector2Int vectorPos in possibleMinesAroundList)
            {
                Tile mine = boardParent.boardDataBase[vectorPos.x, vectorPos.y];

                mine.backImg.sprite = mine.selectedTileImg;
                yield return new WaitForSeconds(boardParent.gameController.botSpeedValue);

                mine.LeftClick();
                boardParent.boardChange = true;
            }

            checkComplete = true;
        }
        else
        {
            if (possibleMinesAroundList.Count + mines == minesAround)
            {
                foreach (Vector2Int vectorPos in possibleMinesAroundList)
                {
                    Tile mine = boardParent.boardDataBase[vectorPos.x, vectorPos.y];

                    mine.state = State.IsMine;
                    mine.backImg.sprite = mine.flagImg;

                    boardParent.isMinesList.Add(this);
                    boardParent.UpdateCounter();

                    if (boardParent.isGameOver) break;
                }

                yield return new WaitForSeconds(boardParent.gameController.botSpeedValue);
            }
        }
    }

    #endregion
}
