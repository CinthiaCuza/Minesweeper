using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Unknown,
    Known,
    PossibleMine,

}

public class Tile : MonoBehaviour
{
    public State state;
    public Vector2Int pos;
    public int minesAround;

    public bool isMine;
}
