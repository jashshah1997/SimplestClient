using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToeController : MonoBehaviour
{
    private TileController[] tiles;
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

    public void SetPlayerType(int playerType)
    {
        foreach(TileController tile in tiles)
        {
            tile.playerType = playerType;
        }
    }
}

public static class PlayerType
{
    public const int CROSS = SessionStartedResponses.CrossesPlayer;
    public const int CIRCLE = SessionStartedResponses.CirclesPlayer;
    public const int EMPTY = -1;
}