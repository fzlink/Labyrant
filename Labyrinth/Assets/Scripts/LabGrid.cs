using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabGrid
{
    public int[,] grid { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public float cellSize { get; set; }

    public LabGrid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        grid = new int[width, height];
    }

    public Vector2Int GetGridPos(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int y = Mathf.FloorToInt(worldPosition.y / cellSize);
        return new Vector2Int(x,y);
    }

    public void SetValue(int x, int y, int value)
    {
        if(x >=0 && y>=0 && x<width && y < height)
        {
            grid[x, y] = value;
        }
    }

    public void SetValue(Vector2Int pos, int value)
    {
        if(pos.x >= 0 && pos.y >= 0 && pos.x <width && pos.y < height)
        {
            grid[pos.x, pos.y] = value;
        }
    }

}
