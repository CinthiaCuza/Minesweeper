using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    [SerializeField] public Board board;

    [SerializeField]  public GameObject PlayButton;
    [SerializeField]  public GameObject StopButton;

    private void Start()
    {
        GameController.GameOverEvent.AddListener(StopBot);
        GameController.RestartEvent.AddListener(StopBot);
    }

    public void StopBot()
    {
        board.botPlaying = false;
        PlayButton.SetActive(true);
        StopButton.SetActive(false);
        StopAllCoroutines();
    }
    public void PlayBot()
    {
        if (!board.isGameOver)
        {
            board.botPlaying = true;
            PlayButton.SetActive(false);
            StopButton.SetActive(true);
            StartCoroutine(InitBotPlaying());
        }
    }

    private IEnumerator InitBotPlaying()
    {
        for (int i = 0; i < board.possibleMinesList.Count; i++)
        {
            Tile tile = board.possibleMinesList[i];

            tile.state = State.Unknown;
            tile.backImg.sprite = tile.backImgsArray[0];
        }

        board.possibleMinesList.Clear();
        board.UpdateCounter();

        if (board.tileWithNumberList.Count == 0)
        {
            Tile tileToShow = board.boardDataBase[Random.Range(0, board.maxrows), Random.Range(0, board.maxcolumns)];
            tileToShow.backImg.sprite = tileToShow.backImgsArray[2];

            yield return new WaitForSeconds(0.5f);

            tileToShow.LeftClick();
            board.boardChange = true;
        }

        yield return new WaitForSeconds(1f);
        StartCoroutine(BotPlaying());
    }

    private IEnumerator BotPlaying()
    {
        for (int i = 0; i < board.tileWithNumberList.Count; i++)
        {
            Tile tile = board.tileWithNumberList[i];
            if (!tile.checkComplete) StartCoroutine(tile.FindMinesAround());
            if (board.isGameOver) break;
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < board.isMinesList.Count; i++)
        {
            Tile mine = board.isMinesList[i];

            for (int j = 0; j < mine.posAroundList.Count; j++)
            {
                Tile tileAround = mine.boardParent.boardDataBase[mine.posAroundList[j].x, mine.posAroundList[j].y];
                if (tileAround.state.Equals(State.Known) && !tileAround.checkComplete) StartCoroutine(tileAround.FindMinesAround());
            }

            if (board.isGameOver) break;
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);

        if (board.boardChange)
        {
            board.boardChange = false;
            StartCoroutine(BotPlaying());
        }
        else
        {
            StopBot();
        }
    }
}
