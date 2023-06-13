using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject tile;
    public int tilesAmount;
    public int minesAmount;

    void Start()
    {
        StartBoard();
    }

    public void StartBoard()
    {
        for (int i = 0; i < tilesAmount; i++) Instantiate(tile, transform);

        List<int> minesPos = new List<int>();

        while (minesPos.Count < minesAmount)
        {
            int randomPos = Random.Range(0, tilesAmount);

            if (!minesPos.Contains(randomPos))
            {
                minesPos.Add(randomPos);

                GameObject tileWithMine = transform.GetChild(randomPos).gameObject;
                tileWithMine.GetComponent<Tile>().isMine = true;
            }
        }
    }
}