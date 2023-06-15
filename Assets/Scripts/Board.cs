using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject tile;
    public Tile[,] boardDataBase;

    [HideInInspector] public int maxTiles;
    [HideInInspector] public int maxMines;

    [HideInInspector] public int rows;
    [HideInInspector] public int columns;

    public void CreateBoard()
    {
        DifficultyData();

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

    public void DifficultyData()
    {
        if (GameController.instance.difficulty == 0)
        {
            rows = columns = 8;
            maxTiles = rows * columns;
            maxMines = 10;
        }
        else if (GameController.instance.difficulty == 1)
        {
            rows = columns = 16;
            maxTiles = rows * columns;
            maxMines = 40;
        }
        else if (GameController.instance.difficulty == 2)
        {
            rows = 30;
            columns = 16;
            maxTiles = 480;
            maxMines = 99;
        }
    }
}