using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;
    public GameController controllerClass;

    public GameObject tile;
    public Tile[,] boardDataBase;

    [HideInInspector] public int maxTiles;
    [HideInInspector] public int maxMines;

    [HideInInspector] public int rows;
    [HideInInspector] public int columns;


    private void Awake()
    {
        instance = this;
    }

    public void LevelSettings(int levelSelected)
    {
        if (levelSelected == 0)
        {
            rows = columns = 8;
            maxTiles = rows * columns;
            maxMines = 10;
        }
        else if (levelSelected == 1)
        {
            rows = columns = 16;
            maxTiles = rows * columns;
            maxMines = 40;
        }
        else if (levelSelected == 2)
        {
            rows = 30;
            columns = 16;
            maxTiles = 480;
            maxMines = 99;
        }

        controllerClass.textRemainingMines.text = maxMines.ToString();
        CreateBoard();
    }

    public void CreateBoard()
    {
        boardDataBase = new Tile[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                GameObject newTile = Instantiate(tile, transform);

                Vector2Int posNewTile = new Vector2Int(i, j);
                newTile.transform.GetComponent<Tile>().pos = posNewTile;

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

                GameObject tileWithMine = transform.GetChild(randomPos).gameObject;
                tileWithMine.GetComponent<Tile>().isMine = true;
            }
        }
    }
}