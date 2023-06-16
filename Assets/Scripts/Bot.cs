using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public Board board;

    public void PlayBot()
    {
        /*while ()
        {

        }*/
        FindAllCurrentMines();
        FindAllSaveTiles();
    }
    public void FindAllCurrentMines()
    {
        for (int i = 0; i < board.possibleMinesList.Count; i++)
        {
            Tile tile = board.possibleMinesList[i];
            tile.state = State.Unknown;
            tile.backImg.sprite = tile.backImgsArray[0];
            tile.boardParent.UpdateCounter(-1);
        }

        board.possibleMinesList.Clear();

        for (int i = 0; i < board.tileWithNumberList.Count; i++)
        {
            board.tileWithNumberList[i].FindMinesAround();
        }
    }

    public void FindAllSaveTiles()
    {
        for (int i = 0; i < board.isMinesList.Count; i++)
        {
            board.isMinesList[i].ShowSaveTilesAround();
        }
    }
}
