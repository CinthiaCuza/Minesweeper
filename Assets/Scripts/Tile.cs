using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum State
{
    Unknown,
    Known,
    PossibleMine,

}

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public bool isMine;

    public Image backImg;
    public Sprite[] backImgs = new Sprite[2];

    public State state;

    public TMP_Text textMinesAround;

    public Vector2Int pos;
    public int minesAround;

    public Image mineImg;
    public Sprite[] mineImgs = new Sprite[2];

    private bool isGameOver;
    Color numberColor = new Color();

    void Start()
    {
        GameController.GameOverEvent.AddListener(ShowMine);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isGameOver)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (state.Equals(State.Unknown))
                {
                    backImg.enabled = false;
                    state = State.Known;

                    if (isMine)
                    {
                        mineImg.sprite = mineImgs[0];
                        mineImg.gameObject.SetActive(true);

                        GameController.GameOverEvent.Invoke();
                    }
                    else
                    {
                        CalculateMinesAround();
                    }
                }
            }

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (state.Equals(State.Unknown))
                {
                    state = State.PossibleMine;
                    backImg.sprite = backImgs[1];
                }
                else if (state.Equals(State.PossibleMine))
                {
                    state = State.Unknown;
                    backImg.sprite = backImgs[0];
                }

                Board.instance.controllerClass.UpdateCounter();
            }
        }
    }

    public void CalculateMinesAround()
    {
        AuxCalculateMinesAround(pos.x - 1, pos.y - 1);
        AuxCalculateMinesAround(pos.x - 1, pos.y);
        AuxCalculateMinesAround(pos.x - 1, pos.y + 1);

        AuxCalculateMinesAround(pos.x, pos.y - 1);
        AuxCalculateMinesAround(pos.x, pos.y + 1);

        AuxCalculateMinesAround(pos.x + 1, pos.y - 1);
        AuxCalculateMinesAround(pos.x + 1, pos.y);
        AuxCalculateMinesAround(pos.x + 1, pos.y + 1);

        if (minesAround != 0) NumberColor();
    }

    public void AuxCalculateMinesAround(int posF, int posC)
    {
        int rows = Board.instance.rows;
        int columns = Board.instance.columns;

        if (posF >= 0 && posF < rows && posC >= 0 && posC < columns)
        {
            if (Board.instance.boardDataBase[posF, posC].isMine) minesAround++;
        }
    }

    public void NumberColor()
    {
        
        if (minesAround == 1) ColorUtility.TryParseHtmlString("#0B24FB", out numberColor);
        else if (minesAround == 2) ColorUtility.TryParseHtmlString("#0E7A11", out numberColor);
        else if (minesAround == 3) ColorUtility.TryParseHtmlString("#FC0D1B", out numberColor);
        else if (minesAround == 4) ColorUtility.TryParseHtmlString("#020B79", out numberColor);
        else if (minesAround == 5) ColorUtility.TryParseHtmlString("#852123", out numberColor);
        else if (minesAround == 6) ColorUtility.TryParseHtmlString("#278786", out numberColor);
        else if (minesAround == 7) ColorUtility.TryParseHtmlString("#000000", out numberColor);
        else if (minesAround == 8) ColorUtility.TryParseHtmlString("#878787", out numberColor);

        textMinesAround.color = numberColor;
        textMinesAround.text = minesAround.ToString();
    }

    public void ShowMine()
    {
        isGameOver = true;

        if (isMine && mineImg.sprite == null)
        {
            backImg.enabled = false;
            mineImg.sprite = mineImgs[1];
            mineImg.gameObject.SetActive(true);
        }
    }
}
