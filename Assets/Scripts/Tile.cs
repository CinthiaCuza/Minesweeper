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
    public bool isMine;

    public Image backImg;
    public Sprite[] backImgsArray = new Sprite[2];

    public State state;

    public TMP_Text textMinesAround;

    public Vector2Int pos;
    public int minesAround;

    public Image mineImg;
    public Sprite[] mineImgsArray = new Sprite[2];

    private bool isGameOver;
    Color numberColor = new Color();

    public List<Vector2Int> posAroundList = new List<Vector2Int>();

    public bool minesFounded;

    void Start()
    {
        GameController.GameOverEvent.AddListener(ShowMine);
        GameController.RestartEvent.AddListener(DestroyTile);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isGameOver)
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

                boardParent.restartEmojiImg.sprite = boardParent.gameController.emojisArray[1];
                GameController.GameOverEvent.Invoke();
            }
            else
            {
                CalculateMinesAround();
            }
        }
    }

    public void RightClick()
    {
        if (state.Equals(State.Unknown))
        {
            state = State.PossibleMine;
            backImg.sprite = backImgsArray[1];

            boardParent.UpdateCounter(1);
            boardParent.possibleMinesList.Add(this);
        }
        else if (state.Equals(State.PossibleMine))
        {
            state = State.Unknown;
            backImg.sprite = backImgsArray[0];

            boardParent.UpdateCounter(-1);
            boardParent.possibleMinesList.Remove(this);
        }
    }

    public void CalculatePosAround()
    {
        int rows = boardParent.rows;
        int columns = boardParent.columns;

        if (pos.x - 1 >= 0 && pos.x - 1 < rows && pos.y - 1 >= 0 && pos.y - 1 < columns)
            posAroundList.Add(new Vector2Int(pos.x - 1, pos.y - 1));

        if (pos.x - 1 >= 0 && pos.x - 1 < rows && pos.y >= 0 && pos.y < columns)
            posAroundList.Add(new Vector2Int(pos.x - 1, pos.y));

        if (pos.x - 1 >= 0 && pos.x - 1 < rows && pos.y + 1 >= 0 && pos.y + 1 < columns)
            posAroundList.Add(new Vector2Int(pos.x - 1, pos.y + 1));

        if (pos.x >= 0 && pos.x < rows && pos.y - 1 >= 0 && pos.y - 1 < columns)
            posAroundList.Add(new Vector2Int(pos.x, pos.y - 1));

        if (pos.x >= 0 && pos.x < rows && pos.y + 1 >= 0 && pos.y + 1 < columns)
            posAroundList.Add(new Vector2Int(pos.x, pos.y + 1));

        if (pos.x + 1 >= 0 && pos.x + 1 < rows && pos.y - 1 >= 0 && pos.y - 1 < columns)
            posAroundList.Add(new Vector2Int(pos.x + 1, pos.y - 1));

        if (pos.x + 1 >= 0 && pos.x + 1 < rows && pos.y >= 0 && pos.y < columns)
            posAroundList.Add(new Vector2Int(pos.x + 1, pos.y));

        if (pos.x + 1 >= 0 && pos.x + 1 < rows && pos.y + 1 >= 0 && pos.y + 1 < columns)
            posAroundList.Add(new Vector2Int(pos.x + 1, pos.y + 1));
    }

    public void CalculateMinesAround()
    {
        for (int i = 0; i < posAroundList.Count; i++)
        {
            if (boardParent.boardDataBase[posAroundList[i].x, posAroundList[i].y].isMine) minesAround++;
        }

        if (minesAround != 0) NumberColor();
        else ExpandMap();
    }

    public void NumberColor()
    {
        
        if (minesAround == 1) ColorUtility.TryParseHtmlString("#0B24FB", out numberColor);
        else if (minesAround == 2) ColorUtility.TryParseHtmlString("#0E7A11", out numberColor);
        else if (minesAround == 3) ColorUtility.TryParseHtmlString("#FC0D1B", out numberColor);
        else if (minesAround == 4) ColorUtility.TryParseHtmlString("#020B79", out numberColor);
        else if (minesAround == 5) ColorUtility.TryParseHtmlString("#852123", out numberColor);
        else if (minesAround == 6) ColorUtility.TryParseHtmlString("#278786", out numberColor);
        else if (minesAround == 7) ColorUtility.TryParseHtmlString("#000000", out numberColor);
        else if (minesAround == 8) ColorUtility.TryParseHtmlString("#878787", out numberColor);

        textMinesAround.color = numberColor;
        textMinesAround.text = minesAround.ToString();
        boardParent.tileWithNumberList.Add(this);
    }

    public void ShowMine()
    {
        isGameOver = true;

        if (isMine && mineImg.sprite == null && !state.Equals(State.PossibleMine))
        {
            backImg.enabled = false;
            mineImg.sprite = mineImgsArray[1];
            mineImg.gameObject.SetActive(true);
        }
    }

    public void ExpandMap()
    {
        for (int i = 0; i < posAroundList.Count; i++)
        {
            Tile tileAround = boardParent.boardDataBase[posAroundList[i].x, posAroundList[i].y];
            if (!tileAround.isMine) tileAround.LeftClick();
        }
    }

    public void DestroyTile()
    {
        Destroy(gameObject);
    }

    public void FindMinesAround() 
    {
        List<Vector2Int> possibleMinesList = new List<Vector2Int>();

        for (int i = 0; i < posAroundList.Count; i++) 
        {
            Tile tileAround = boardParent.boardDataBase[posAroundList[i].x, posAroundList[i].y];

            if (tileAround.state.Equals(State.Unknown) || tileAround.state.Equals(State.IsMine))
            {
                possibleMinesList.Add(tileAround.pos);
            }
        }

        if(possibleMinesList.Count == minesAround)
        {
            for (int i = 0; i < possibleMinesList.Count; i++)
            {
                Tile mine = boardParent.boardDataBase[possibleMinesList[i].x, possibleMinesList[i].y];

                if (!mine.state.Equals(State.IsMine))
                {
                    mine.state = State.IsMine;
                    mine.backImg.sprite = mine.backImgsArray[1];

                    mine.boardParent.UpdateCounter(1);
                    mine.boardParent.isMinesList.Add(this);
                }
            }

            minesFounded = true;
        }
    }

    public void ShowSaveTilesAround() 
    {
        for (int i = 0; i < posAroundList.Count; i++) 
        {
            Tile tileAround = boardParent.boardDataBase[posAroundList[i].x, posAroundList[i].y];

            if (tileAround.state.Equals(State.Known))
            {
                List<Vector2Int> possibleSaveTilesList = new List<Vector2Int>();
                int mines = 0;

                for (int j = 0; j < tileAround.posAroundList.Count; j++) 
                {
                    Tile possibleSaveTile = boardParent.boardDataBase[tileAround.posAroundList[j].x, tileAround.posAroundList[j].y];

                    if (possibleSaveTile.state.Equals(State.Unknown)) possibleSaveTilesList.Add(possibleSaveTile.pos);
                    if (possibleSaveTile.state.Equals(State.IsMine)) mines++;
                }

                if (mines == tileAround.minesAround)
                {
                    for (int j = 0; j < possibleSaveTilesList.Count; j++)
                    {
                        Tile saveTile = boardParent.boardDataBase[possibleSaveTilesList[j].x, possibleSaveTilesList[j].y];

                        if (saveTile.state.Equals(State.Unknown)) saveTile.LeftClick();
                    }
                }
            }
        }
    }
}
