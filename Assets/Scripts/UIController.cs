using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;
    public static UnityEvent GameOverEvent;

    public Board board;

    public TMP_Text textRemainingMines;

    public int markedMines;

    public Image restartEmojiImg;
    public Sprite[] restartEmojiImgs = new Sprite[2];

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Emoji(0);
        if (GameOverEvent == null) GameOverEvent = new UnityEvent();

        board.CreateBoard();
        textRemainingMines.text = board.maxMines.ToString();
    }

    public void LevelSettings(int difficultySelected)
    {
        GameController.instance.difficulty = difficultySelected;
        RestartGame();
    }

    public void UpdateCounter()
    {
        markedMines++;
        int remainingMines = board.maxMines - markedMines;

        textRemainingMines.text = remainingMines.ToString();
    }

    public void RestartGame()
    {
        markedMines = 0;
        SceneManager.LoadScene(0);
    }

    public void Emoji(int emoji)
    {
        restartEmojiImg.sprite = restartEmojiImgs[emoji];
    }
}
