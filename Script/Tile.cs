using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public int type;
    private Boardmanager boardManager;
    public void Init(int x, int y, Boardmanager boardManager)
    {
        this.x = x;
        this.y = y;
        this.boardManager = boardManager;
    }
    private void OnMouseDown()
    {
        if (boardManager != null)
        {
            boardManager.OnTileClicked(this);
        }
    }
 
}
