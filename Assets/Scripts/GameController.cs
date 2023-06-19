using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

[System.Serializable]
public class BoardsList
{
    public BoardData[] boards;
}

[System.Serializable]
public class BoardData
{
    public int maxrows;
    public int maxcolumns;
    public int maxMines;
    public BoardRows[] boardTiles;
}

[System.Serializable]
public class BoardRows
{
    public int[] row;
}

public class GameController : MonoBehaviour
{
    public GameObject tile;

    public static UnityEvent GameOverEvent;
    public static UnityEvent RestartEvent;

    public GameObject[] boardsArray = new GameObject[3];
    public Sprite[] emojisArray = new Sprite[2];

    public int difficulty = 0;

    public float botSpeedValue;

    [SerializeField] 
    public string filePath = "Assets/Resources/JSONBoard.txt";

    public BoardsList boardJSON = new BoardsList();
    public bool useJSON;
    public GameObject[] loadJSONButtons = new GameObject[3];

    private void Start()
    {
        GameOverEvent = new UnityEvent();
        RestartEvent = new UnityEvent();

        ReadJSON();

        boardsArray[difficulty].SetActive(true);
    }

    private void ReadJSON()
    {
        if (File.Exists(filePath))
        {
            try
            {
                string textJSON = File.ReadAllText(filePath);
                boardJSON = JsonUtility.FromJson<BoardsList>(textJSON);

                if (boardJSON == null)
                {
                    Debug.LogError("The JSON file could not be loaded");
                    RemoveJSONOption();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error while parsing the JSON " + ex.Message);
                RemoveJSONOption();
            }
        }
        else
        {
            Debug.LogError("The file does not exist");
            RemoveJSONOption();
        }
    }

    private void RemoveJSONOption() 
    {
        foreach (GameObject buttonJSON in loadJSONButtons) buttonJSON.SetActive(false);
    }

    public void LevelSettings(int difficultySelected)
    {
        UpdateDifficulty(difficultySelected, false);
    }

    public void LevelSettingsJSON(int difficultySelected)
    {
        UpdateDifficulty(difficultySelected, true);
    }

    private void UpdateDifficulty(int difficultySelected, bool setToUseJSON)
    {
        if (difficulty != difficultySelected)
        {
            useJSON = setToUseJSON;
            RestartEvent.Invoke();

            boardsArray[difficulty].SetActive(false);
            difficulty = difficultySelected;
            boardsArray[difficulty].SetActive(true);

            boardsArray[difficulty].GetComponent<Board>().Restart();
        }
        else
        {
            if (!useJSON && setToUseJSON || useJSON && !setToUseJSON)
            {
                useJSON = setToUseJSON;
                RestartButton();
            }
        }
    }

    public void RestartButton()
    {
        RestartEvent.Invoke();
        boardsArray[difficulty].GetComponent<Board>().Restart();
    }
}
