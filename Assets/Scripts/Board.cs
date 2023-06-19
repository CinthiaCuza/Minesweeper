using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public GameController gameController;
    public GameObject gridBoard;

    [HideInInspector] public int maxTiles;
    [HideInInspector] public int maxMines = 10;
    [HideInInspector] public int maxrows;
    [HideInInspector] public int maxcolumns;

    public Image restartEmojiImg;
    public TMP_Text textRemainingMines;

    [HideInInspector] public Tile[,] boardDataBase;

    [HideInInspector] public List<Tile> possibleMinesList = new List<Tile>();
    [HideInInspector] public List<Tile> isMinesList = new List<Tile>();
    [HideInInspector] public List<Tile> tileWithNumberList = new List<Tile>();

    [HideInInspector] public bool boardChange;
    [HideInInspector] public bool isGameOver;
    [HideInInspector] public bool botPlaying;

    private void Start()
    {
        DifficultyData();
        CreateBoard();
    }

    private void DifficultyData()
    {
        if (gameController.useJSON)
        {
            maxrows = gameController.boardJSON.boards[gameController.difficulty].maxrows;
            maxcolumns = gameController.boardJSON.boards[gameController.difficulty].maxcolumns;
            maxTiles = maxrows * maxcolumns;
            maxMines = gameController.boardJSON.boards[gameController.difficulty].maxMines;
        }
        else
        {
            switch (gameController.difficulty)
            {
                case 0:
                    maxrows = maxcolumns = 8;
                    maxTiles = maxrows * maxcolumns;
                    maxMines = 10;
                    break;
                case 1:
                    maxrows = maxcolumns = 16;
                    maxTiles = maxrows * maxcolumns;
                    maxMines = 40;
                    break;
                case 2:
                    maxrows = 16;
                    maxcolumns = 30;
                    maxTiles = 480;
                    maxMines = 99;
                    break;
            }
        }

        boardDataBase = new Tile[maxrows, maxcolumns];
    }

    private void CreateBoard()
    {
        textRemainingMines.text = maxMines.ToString();

        for (int i = 0; i < maxrows; i++)
        {
            for(int j = 0; j < maxcolumns; j++)
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

            while (minesPosList.Count < maxMines)
            {
                int randomPos = Random.Range(0, maxTiles);

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
        int remainingMines = maxMines - maxFlags;

        if(remainingMines >= 0) textRemainingMines.text = remainingMines.ToString("D2");

        if (isMinesList.Count == maxMines)
        {
            isGameOver = true;
            GameController.GameOverEvent.Invoke();
        }
    }

    public void Restart()
    {
        restartEmojiImg.sprite = gameController.emojisArray[0];
        isGameOver = boardChange = false;

        possibleMinesList.Clear();
        isMinesList.Clear();
        tileWithNumberList.Clear();

        CreateBoard();
    }
}