using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileController : MonoBehaviour, IPointerClickHandler
{
    public int currentState = PlayerType.EMPTY;
    public int playerType = PlayerType.CIRCLE;

    private Sprite emptyImage;
    private Sprite crossImage;
    private Sprite circleImage;
    private Image tileImage;
    
    // Start is called before the first frame update
    void Start()
    {
        tileImage = GetComponent<Image>();
        emptyImage = Resources.Load<Sprite>("Tile_Empty");
        crossImage = Resources.Load<Sprite>("Tile_X");
        circleImage = Resources.Load<Sprite>("Tile_O");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if its my turn
        if (!GetComponentInParent<TicTacToeController>().isMyTurn)
            return;

        ChangeTileState(playerType);

        // My turn finished
        GetComponentInParent<TicTacToeController>().isMyTurn = false;
    }

    private void ChangeTileState(int newState)
    {
        if (newState == PlayerType.CROSS)
        {
            tileImage.sprite = crossImage;
            currentState = PlayerType.CROSS;
        }
        else if (newState == PlayerType.CIRCLE)
        {
            tileImage.sprite = circleImage;
            currentState = PlayerType.CIRCLE;
        }
    }
}
