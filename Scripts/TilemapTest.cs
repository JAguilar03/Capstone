using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// This script demonstrates how to copy and paste a tilemap prefab onto a canvas tilemap,
// aligning the prefab using different corners (top-left, top-right, bottom-left, bottom-right).
// It places the same prefab four times at the same destination position but offsets it differently
// based on the specified corner alignment.

public class TilemapTest : MonoBehaviour
{
    enum TileCorner {
        topLeft,
        topRight,
        bottomLeft,
        bottomRight
    }

    public Tilemap tileCanvas;      //Tilemap to be modified
    public Tilemap tilemapPrefab;   //source Tilemap prefab
    public Vector3Int destinationPos = Vector3Int.zero; //Coordinates to copy prefab to

    // Start is called before the first frame update
    void Start()
    {
        //Place the same prefab at the same position 4 time, but use a different corner each time
        placeTilePrefab(tilemapPrefab, destinationPos, TileCorner.topLeft);
        placeTilePrefab(tilemapPrefab, destinationPos, TileCorner.topRight);
        placeTilePrefab(tilemapPrefab, destinationPos, TileCorner.bottomLeft);
        placeTilePrefab(tilemapPrefab, destinationPos, TileCorner.bottomRight);
    }

    void placeTilePrefab(Tilemap prefab, Vector3Int position, TileCorner corner)
    {
        //Get bounds of prefab
        BoundsInt bounds = tilemapPrefab.cellBounds;
        TileBase[] tiles = tilemapPrefab.GetTilesBlock(bounds);

        //Assume that a tilemap prefab's coordinates "origin" is defined at a corner between 4 tiles
        //  The following cases define which corner of the tilemap is placed at the origin
        Vector3Int cornerOffset = Vector3Int.zero;
        switch (corner)
        {
            case TileCorner.topLeft:
                cornerOffset = new Vector3Int(0, -(bounds.yMax - bounds.yMin), 0);
                break;
            case TileCorner.topRight:
                cornerOffset = new Vector3Int(-(bounds.xMax - bounds.xMin), -(bounds.yMax - bounds.yMin), 0);
                break;
            case TileCorner.bottomLeft:
                cornerOffset = new Vector3Int(0, 0, 0);
                break;
            case TileCorner.bottomRight:
                cornerOffset = new Vector3Int(-(bounds.xMax - bounds.xMin), 0, 0);
                break;

        }

        // Adjust bounds to the destination
        BoundsInt targetBounds = new BoundsInt(destinationPos + cornerOffset, bounds.size);

        // Paste the tiles in one go
        tileCanvas.SetTilesBlock(targetBounds, tiles);
    }
}
