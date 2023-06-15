using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public GameController gameController;
    public GameObject gridBoard;

    public Tile[,] boardDataBase;

    [HideInInspector] public int maxTiles;
    [HideInInspector] public int maxMines;

    [HideInInspector] public int rows;
    [HideInInspector] public int columns;

    public TMP_Text textRemainingMines;

    public int markedMines;

    public Image restartEmojiImg;

    private void Start()
    {
        DifficultyData();

        boardDataBase = new Tile[rows, columns];

        CreateBoard();
    }

    public void CreateBoard()
    {
        for (int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                GameObject newTile = Instantiate(gameController.tile, gridBoard.transform);

                newTile.GetComponent<Tile>().boardParent = this;

                Vector2Int posNewTile = new Vector2Int(i, j);
                newTile.transform.GetComponent<Tile>().pos = posNewTile;
                newTile.transform.GetComponent<Tile>().CalculatePosAround();

                boardDataBase[i, j] = newTile.transform.GetComponent<Tile>();
            }
        }

        List<int> minesPos = new List<int>();

        while (minesPos.Count < maxMines)
        {
            int randomPos = Random.Range(0, maxTiles);

            if (!minesPos.Contains(randomPos))
            {
                minesPos.Add(randomPos);

                Tile tileWithMine = gridBoard.transform.GetChild(randomPos).GetComponent<Tile>();

                tileWithMine.isMine = true;
                boardDataBase[tileWithMine.pos.x, tileWithMine.pos.y].isMine = true;
            }
        }

        textRemainingMines.text = maxMines.ToString();
    }

    public void DifficultyData()
    {
        if (gameController.difficulty == 0)
        {
            rows = columns = 8;
            maxTiles = rows * columns;
            maxMines = 10;
        }
        else if (gameController.difficulty == 1)
        {
            rows = columns = 16;
            maxTiles = rows * columns;
            maxMines = 40;
        }
        else if (gameController.difficulty == 2)
        {
            rows = 30;
            columns = 16;
            maxTiles = 480;
            maxMines = 99;
        }
    }

    public void UpdateCounter(int point)
    {
        markedMines += point;
        int remainingMines = maxMines - markedMines;

        textRemainingMines.text = remainingMines.ToString();
    }

    public void RestartButton()
    { 
        restartEmojiImg.sprite = gameController.restartEmojiImgs[0];
        GameController.RestartEvent.Invoke();

        markedMines = 0;
        CreateBoard();
    }
}