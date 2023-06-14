using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject tile;

    void Start()
    {
        StartBoard();
    }

    public void StartBoard()
    {
        for (int i = 0; i < GameController.instance.maxTiles; i++) Instantiate(tile, transform);

        List<int> minesPos = new List<int>();

        while (minesPos.Count < GameController.instance.maxMines)
        {
            int randomPos = Random.Range(0, GameController.instance.maxTiles);

            if (!minesPos.Contains(randomPos))
            {
                minesPos.Add(randomPos);

                GameObject tileWithMine = transform.GetChild(randomPos).gameObject;
                tileWithMine.GetComponent<Tile>().isMine = true;
            }
        }
    }
}