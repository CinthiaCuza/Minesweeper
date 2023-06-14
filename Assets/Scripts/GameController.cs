using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public TMP_Text textRemainingMines;

    public int maxTiles;
    public int maxMines;
    public int markedMines;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        textRemainingMines.text = maxMines.ToString();
    }

    public void UpdateCounter()
    {
        markedMines++;
        int remainingMines = maxMines - markedMines;

        textRemainingMines.text = remainingMines.ToString();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
