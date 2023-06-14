using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static UnityEvent GameOverEvent;

    public TMP_Text textRemainingMines;

    public int markedMines;

    public Image restartEmojiImg;
    public Sprite[] restartEmojiImgs = new Sprite[2];

    private void Start()
    {
        if (GameOverEvent == null) GameOverEvent = new UnityEvent();
        GameOverEvent.AddListener(GameOverEmoji);
    }

    public void UpdateCounter()
    {
        markedMines++;
        int remainingMines = Board.instance.maxMines - markedMines;

        textRemainingMines.text = remainingMines.ToString();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void GameOverEmoji()
    {
        restartEmojiImg.sprite = restartEmojiImgs[1];
    }
}
