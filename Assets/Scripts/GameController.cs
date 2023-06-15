using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public GameObject tile;

    public static UnityEvent GameOverEvent;
    public static UnityEvent RestartEvent;

    public GameObject[] boards = new GameObject[3];
    public Sprite[] restartEmojiImgs = new Sprite[2];

    public int difficulty = 0;

    private void Start()
    {
        GameOverEvent = new UnityEvent();
        RestartEvent = new UnityEvent();

        boards[difficulty].SetActive(true);
    }

    public void LevelSettings(int difficultySelected)
    {
        if (difficulty != difficultySelected)
        {
            RestartEvent.Invoke();

            boards[difficulty].SetActive(false);
            difficulty = difficultySelected;
            
            boards[difficulty].SetActive(true);
        }
    }
}
