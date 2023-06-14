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

    //public Vector2Int pos;
    //public int minesAround;
    //public Image img;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (state.Equals(State.Unknown))
            {
                backImg.enabled = false;
                state = State.Known;
                textMinesAround.text = 1.ToString();
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

            GameController.instance.UpdateCounter();
        }
    }
}
