using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu()]
public class TileBases : ScriptableObject
{
    public TileBase topLeftCorner;
    public TileBase topRightCorner;
    public TileBase bottomLeftCorner;
    public TileBase bottomRightCorner;
    public TileBase topLeftWall;
    public TileBase topRightWall;
    public TileBase BottomLeftWall;
    public TileBase BottomRightWall;
    public TileBase topMost;
    public TileBase leftMost;
    public TileBase rightMost;
    public TileBase bottomMost;
    public TileBase topWall;
    public TileBase leftWall;
    public TileBase rightWall;
    public TileBase bottomWall;
    public TileBase pathTopLeft;
    public TileBase pathTop;
    public TileBase pathTopRight;
    public TileBase pathLeft;
    public TileBase pathBottomLeft;
    public TileBase pathBottom;
    public TileBase pathBottomRight;
    public TileBase pathRight;
    public TileBase pathCenter;
}
