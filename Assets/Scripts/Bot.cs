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
           /* for (int i = 0; i < board.tileWithNumberList.Count; i++)
            {
                Tile tile = board.tileWithNumberList[i];
                if (!tile.checkComplete) FindSaveTiles(tile);
            }*/
            
            StopBot();
        }
    }

    private void FindSaveTiles(Tile tile)
    {
        List<Vector2Int> possibleMinesAroundList = new List<Vector2Int>();
        List<Vector2Int> mines = new List<Vector2Int>();

        for (int i = 0; i < tile.posAroundList.Count; i++)
        {
            Tile tileAround = tile.boardParent.boardDataBase[tile.posAroundList[i].x, tile.posAroundList[i].y];

            if (tileAround.state.Equals(State.Unknown)) possibleMinesAroundList.Add(tileAround.pos);
            else if (tileAround.state.Equals(State.IsMine)) mines.Add(tileAround.pos);
        }

        if (possibleMinesAroundList.Count + mines.Count > tile.minesAround)
        {
            int minesToPlace = tile.minesAround - mines.Count;

            if(minesToPlace < possibleMinesAroundList.Count && minesToPlace != 1)
            {
                int[] elements = new int[possibleMinesAroundList.Count];
                int[] positions = new int[possibleMinesAroundList.Count];

                for (int i = 0; i < minesToPlace; i++) elements[i] = i;
                for (int i = 0; i < possibleMinesAroundList.Count; i++) positions[i] = i;

                Dictionary<int, int> repeatingElements = FindRepeatingElement(elements, positions);

                if (repeatingElements.Count != 0)
                {
                    foreach (var kvp in repeatingElements)
                    {
                        Tile tileToClick = board.boardDataBase[possibleMinesAroundList[kvp.Key].x, possibleMinesAroundList[kvp.Key].y];
                        tileToClick.LeftClick();
                    }
                }
            }
        }
    }

    public static Dictionary<int, int> FindRepeatingElement(int[] elements, int[] positions)
    {
        Dictionary<int, int> repeatingElements = new Dictionary<int, int>();

        List<List<int>> variants = new List<List<int>>();
        FindVariantsHelper(elements, positions, 0, new List<int>(), variants);

        foreach (int position in positions)
        {
            int repeatingElement = variants[0][position - 1];
            bool isRepeating = true;

            for (int i = 1; i < variants.Count; i++)
            {
                if (variants[i][position - 1] != repeatingElement)
                {
                    isRepeating = false;
                    break;
                }
            }

            if (isRepeating)
            {
                repeatingElements[position] = repeatingElement;
            }
        }

        return repeatingElements;
    }

    private static void FindVariantsHelper(int[] elements, int[] positions, int index, List<int> currentVariant, List<List<int>> variants)
    {
        if (index == elements.Length)
        {
            variants.Add(new List<int>(currentVariant));
            return;
        }

        foreach (int position in positions)
        {
            if (!currentVariant.Contains(position))
            {
                currentVariant.Add(position);
                FindVariantsHelper(elements, positions, index + 1, currentVariant, variants);
                currentVariant.RemoveAt(currentVariant.Count - 1);
            }
        }
    }
}
