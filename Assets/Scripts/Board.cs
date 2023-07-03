using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public struct BoardConfiguration
{
    public int maxTiles;
    public int maxMines;
    public int maxrows;
    public int maxcolumns;
}

public class Board : MonoBehaviour
{
    public GameController gameController;
    public GameObject gridBoard;

    public BoardConfiguration boardConf;

    public Image restartEmojiImg;
    public TMP_Text textRemainingMines;

    public Tile[,] boardDataBase;

    public List<Tile> possibleMinesList = new List<Tile>();
    public List<Tile> isMinesList = new List<Tile>();
    public List<Tile> tileWithNumberList = new List<Tile>();

    public bool boardChange;
    public bool isGameOver;
    public bool botPlaying;

    private void Start()
    {
        DifficultyData();
        CreateBoard();
    }

    private void DifficultyData()
    {
        if (gameController.useJSON)
        {
            boardConf.maxrows = gameController.boardJSON.boards[gameController.difficulty].maxrows;
            boardConf.maxcolumns = gameController.boardJSON.boards[gameController.difficulty].maxcolumns;
            boardConf.maxTiles = boardConf.maxrows * boardConf.maxcolumns;
            boardConf.maxMines = gameController.boardJSON.boards[gameController.difficulty].maxMines;

        }
        else
        {
            switch (gameController.difficulty)
            {
                case 0:
                    boardConf.maxrows = boardConf.maxcolumns = 8;
                    boardConf.maxTiles = boardConf.maxrows * boardConf.maxcolumns;
                    boardConf.maxMines = 10;
                    break;
                case 1:
                    boardConf.maxrows = boardConf.maxcolumns = 16;
                    boardConf.maxTiles = boardConf.maxrows * boardConf.maxcolumns;
                    boardConf.maxMines = 40;
                    break;
                case 2:
                    boardConf.maxrows = 16;
                    boardConf.maxcolumns = 30;
                    boardConf.maxTiles = 480;
                    boardConf.maxMines = 99;
                    break;
            }
        }

        boardDataBase = new Tile[boardConf.maxrows, boardConf.maxcolumns];
    }

    private void CreateBoard()
    {
        textRemainingMines.text = boardConf.maxMines.ToString();

        for (int i = 0; i < boardConf.maxrows; i++)
        {
            for(int j = 0; j < boardConf.maxcolumns; j++)
            {
                GameObject newTile = Instantiate(gameController.tile, gridBoard.transform);
                
                Tile tileClass = newTile.GetComponent<Tile>();
                Vector2Int posNewTile = new Vector2Int(i, j);

                tileClass.boardParent = this;
                tileClass.pos = posNewTile;
                tileClass.CalculatePosAround();

                if (gameController.useJSON)
                {
                    if(gameController.boardJSON.boards[gameController.difficulty].boardTiles[i].row[j] == -1)
                        tileClass.isMine = true;
                }

                boardDataBase[i, j] = tileClass;
            }
        }

        if (!gameController.useJSON)
        {
            List<int> minesPosList = new List<int>();

            while (minesPosList.Count < boardConf.maxMines)
            {
                int randomPos = Random.Range(0, boardConf.maxTiles);

                if (!minesPosList.Contains(randomPos))
                {
                    minesPosList.Add(randomPos);

                    Tile tileWithMine = gridBoard.transform.GetChild(randomPos).GetComponent<Tile>();
                    boardDataBase[tileWithMine.pos.x, tileWithMine.pos.y].isMine = true;
                }
            }
        }
    }

    public void UpdateCounter()
    {
        int maxFlags = possibleMinesList.Count + isMinesList.Count;
        int remainingMines = boardConf.maxMines - maxFlags;

        if(remainingMines >= 0) textRemainingMines.text = remainingMines.ToString("D2");

        if (isMinesList.Count == boardConf.maxMines)
        {
            isGameOver = true;
            GameController.GameOverEvent.Invoke();
        }
    }

    public void Restart()
    {
        restartEmojiImg.sprite = gameController.emojiSmile;
        isGameOver = boardChange = false;

        possibleMinesList.Clear();
        isMinesList.Clear();
        tileWithNumberList.Clear();

        CreateBoard();
    }
}