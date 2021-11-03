using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class TicTacToeController : MonoBehaviour
{
    private TileController[] tiles;
    public bool isMyTurn = false;

    // Start is called before the first frame update
    void Start()
    {
        tiles = GetComponentsInChildren<TileController>();
        foreach (TileController tile in tiles)
        {
            Debug.Log(tile.gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int CheckForWinner()
    {
        int check = CheckForWinnerHelper(0, 1, 2);
        if (check != PlayerType.EMPTY) return check;

        check = CheckForWinnerHelper(3, 4, 5);
        if (check != PlayerType.EMPTY) return check;

        check = CheckForWinnerHelper(6, 7, 8);
        if (check != PlayerType.EMPTY) return check;

        check = CheckForWinnerHelper(0, 3, 6);
        if (check != PlayerType.EMPTY) return check;

        check = CheckForWinnerHelper(1, 4, 7);
        if (check != PlayerType.EMPTY) return check;

        check = CheckForWinnerHelper(2, 5, 8);
        if (check != PlayerType.EMPTY) return check;

        check = CheckForWinnerHelper(0, 4, 8);
        if (check != PlayerType.EMPTY) return check;

        check = CheckForWinnerHelper(2, 4, 6);
        if (check != PlayerType.EMPTY) return check;

        return PlayerType.EMPTY;
    }

    private int CheckForWinnerHelper(int idx1, int idx2, int idx3)
    {
        if (
            tiles[idx1].currentState == tiles[idx2].currentState && 
            tiles[idx2].currentState == tiles[idx3].currentState &&
            tiles[idx1].currentState != PlayerType.EMPTY)
        {
            return tiles[idx1].currentState;
        }

        return PlayerType.EMPTY;
    }

    public void SetPlayerType(int playerType)
    {
        foreach(TileController tile in tiles)
        {
            tile.playerType = playerType;
        }
    }

    public string SerializeGameState()
    {
        StringBuilder sb = new StringBuilder();
        foreach(TileController tile in tiles)
        {
            sb.Append(tile.currentState);
        }
        return sb.ToString();
    }

    public void SetGameState(string serializedGameState)
    {
        if (serializedGameState.Length != tiles.Length)
        {
            Debug.LogError("Something has gone wrong! Tile size different from serialized state!");
            return;
        }

        for (int i = 0; i < serializedGameState.Length; i++)
        {
            tiles[i].ChangeTileState(int.Parse(serializedGameState[i].ToString()));
        }
    }
}

public static class PlayerType
{
    public const int CROSS = SessionStartedResponses.CrossesPlayer;
    public const int CIRCLE = SessionStartedResponses.CirclesPlayer;
    public const int EMPTY = 0;
}