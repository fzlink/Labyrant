﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum Directions
{
    L=1,
    U=2,
    R=3,
    D=4
}


public class Labyrinth : MonoBehaviour
{
    public Tilemap tileMap;
    public TileBase pathTile;
    public TileBase wallTile;
    public TileBase characterTile;
    public TileBase finishTile;

    public static Labyrinth instance;

    public int size;
    bool IsAlgorithmRunning;

    LabGrid labGrid;

    Vector3 mouseWorldPos;
    Vector3Int tileCoordinate;

    private Vector3Int obstacleOffset;

    bool isClicked;
    private Vector3Int lastHoverCoordinate;


    public InputField sizeInputField;
    public InputField randomObstacleNumField;
    public Toggle horizontalToggle;
    public Toggle verticalToggle;
    public Text message;
    public TMP_Text generationNumber;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GenerateMap();

        verticalToggle.isOn = true;
        obstacleOffset = new Vector3Int(0, 1, 0);

    }

    private void GenerateMap()
    {
        TileBase[] tileBases = new TileBase[size * size];
        labGrid = new LabGrid(size, size, 1f);
        //int row = 0;
        //for (int i = 0; i < tileBases.Length; i++)
        //{
        //    if (i < size || i == size * (row - 1) || i == size * row - 1 || i > size * size - size)
        //    {
        //        tileBases[i] = wallTile;
        //        labGrid.grid[i / size, i - size * row] = 1;
        //    }
        //    else if (i == size * 2 - 2)
        //        tileBases[i] = finishTile;
        //    else if (i == size * size - size * 2 + 1)
        //        tileBases[i] = characterTile;
        //    else
        //        tileBases[i] = pathTile;
        //    if (i == size * row) row++;
        //}

        tileMap.size = new Vector3Int(size, size, 1);
        tileMap.origin = new Vector3Int(0, 0, 0);
        tileMap.ResizeBounds();
        BoundsInt bounds = tileMap.cellBounds;
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                if(x == 0 || y == 0 || x == size-1 || y == size-1)
                {
                    tileBases[x + y * bounds.size.x] = wallTile;
                    labGrid.grid[x, y] = 1;
                }
                else if(x == size-2 && y == 1)
                {
                    tileBases[x + y * bounds.size.x] = finishTile;
                    labGrid.grid[x, y] = 2;
                }
                else if(x==1 && y == size - 2)
                {
                    tileBases[x + y * bounds.size.x] = characterTile;
                }
                else
                {
                    tileBases[x + y * bounds.size.x] = pathTile;
                }
            }
        }


        tileMap.SetTilesBlock(tileMap.cellBounds, tileBases);
        DeleteTileMapFlags();
        Camera.main.transform.position = new Vector3(tileMap.size.x / 2, tileMap.size.y / 2, -10);
        Camera.main.orthographicSize = tileMap.size.x / 2 + tileMap.size.x / 4;
    }

    private void DeleteTileMapFlags()
    {
        Vector3Int coord = Vector3Int.zero;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                coord.x = i;
                coord.y = j;
                tileMap.SetTileFlags(coord, TileFlags.None);
            }
        }
    }

    private void Update()
    {
        if (IsAlgorithmRunning) return;

        if (Input.GetMouseButton(0))
            isClicked = true;
        else
            isClicked = false;


        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tileCoordinate = tileMap.WorldToCell(mouseWorldPos);
        //Debug.Log(tileCoordinate);
        if(tileCoordinate.x >=0 && tileCoordinate.y >= 0 && tileCoordinate.x < size && tileCoordinate.y < size)
        {
            bool isHitWall = false;
            for (int j = 0; j < 4; j++)
            {
                if (CheckForObstacle(tileCoordinate,j))
                {
                    isHitWall = true;
                    break;
                }
            }

            if (!isHitWall)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (isClicked)
                    {
                        tileMap.SetTile(tileCoordinate + obstacleOffset * i, wallTile);
                        labGrid.grid[(tileCoordinate + obstacleOffset * i).x, (tileCoordinate + obstacleOffset * i).y] = 1;
                    }
                    else
                    {
                        //tileMap.SetTileFlags(tileCoordinate + obstacleOffset * i, TileFlags.None);
                        tileMap.SetColor(tileCoordinate + obstacleOffset * i, Color.blue);
                    }
                }
            }
        }

        
        if (lastHoverCoordinate != tileCoordinate)
        {
            for (int i = 0; i < 4; i++)
            {
                tileMap.SetTileFlags(lastHoverCoordinate + obstacleOffset * i, TileFlags.None);
                tileMap.SetColor(lastHoverCoordinate + obstacleOffset * i, Color.white);
            }
        }
        lastHoverCoordinate = tileCoordinate;
    }

    private bool CheckForObstacle(Vector3Int coord, int j)
    {
        bool isStart = tileMap.GetTile(coord + obstacleOffset * j) == characterTile;
        bool isEnd = tileMap.GetTile(coord + obstacleOffset * j) == finishTile;
        bool isWall = tileMap.GetTile(coord + obstacleOffset * j) == wallTile;
        bool isNull = tileMap.GetTile(coord + obstacleOffset * j) == null;
        return isStart | isEnd | isWall | isNull;
    }

    public void SetLabyrinthSize()
    {
        size = Convert.ToInt32(sizeInputField.text);
        GenerateMap();
    }

    public void OnObstacleHorizontal()
    {
        obstacleOffset = new Vector3Int(1, 0, 0);
    }
    public void OnObstacleVertical()
    {
        obstacleOffset = new Vector3Int(0, 1, 0);
    }

    public void PlaceRandomObstacles()
    {
        GenerateMap();
        int totalObstacleNum = Convert.ToInt32(randomObstacleNumField.text);
        if(totalObstacleNum > size*2)
        {
            randomObstacleNumField.text = "";
            randomObstacleNumField.placeholder.GetComponent<Text>().text = "Must be less than size";
            randomObstacleNumField.placeholder.color = Color.red;
            return;
        }
        else
        {
            randomObstacleNumField.text = "";
            randomObstacleNumField.placeholder.GetComponent<Text>().text = "Random Obstacle Number";
            randomObstacleNumField.placeholder.color = Color.gray;
        }

        int placedObstacleNum = 0;
        int x;
        int y;
        bool isHit = false;
        Vector3Int coord = Vector3Int.zero;
        while (placedObstacleNum < totalObstacleNum)
        {
            if(UnityEngine.Random.value < 0.5f)
                obstacleOffset = new Vector3Int(0, 1, 0);
            else
                obstacleOffset = new Vector3Int(1, 0, 0);

            x = UnityEngine.Random.Range(1, size - 1);
            y = UnityEngine.Random.Range(1, size - 1);
            coord.x = x;
            coord.y = y;

            isHit = false;
            for (int j = 0; j < 4; j++)
            {
                if(CheckForObstacle(coord,j))
                {
                    isHit = true;
                    break;
                }
            }
            if (!isHit)
            {
                for (int k = 0; k < 4; k++)
                {
                    tileMap.SetTile(coord + obstacleOffset * k, wallTile);
                    labGrid.grid[(coord + obstacleOffset * k).x, (coord + obstacleOffset * k).y] = 1;
                }
                placedObstacleNum++;
            }
        }
        verticalToggle.isOn = true;
        obstacleOffset = new Vector3Int(0, 1, 0);

    }

    public void Run()
    {
        message.gameObject.SetActive(false);
        Vector2Int start = new Vector2Int(1, size - 2);
        Vector2Int finish = new Vector2Int(size - 2, 1);
        if (!Astar.FindPath(labGrid, start, finish))
        {
            message.gameObject.SetActive(true);
            message.text = "NO PATH AVAILABLE";
            message.color = Color.red;
            return;
        }
        IsAlgorithmRunning = true;
        bool isTimeout;
        List<Child> bestOfGeneration = GAController.GeneticAlgorithm(labGrid.grid,size, start, finish, out isTimeout);
        if(bestOfGeneration != null)
        {
            message.gameObject.SetActive(true);
            message.text = "PATH FOUND";
            message.color = Color.blue;

            StartCoroutine(ShowGenerations(bestOfGeneration, start, finish));
        }
        else if (isTimeout)
        {
            message.gameObject.SetActive(true);
            message.text = "TIMEOUT";
            message.color = Color.blue;
        }


    }

    private IEnumerator ShowGenerations(List<Child> bestOfGeneration, Vector2Int start, Vector2Int finish)
    {
        List<Vector3Int> coloredPath;
        for (int i = 0; i < bestOfGeneration.Count; i++)
        {
            coloredPath = ShowPath(bestOfGeneration[i], start, finish);
            generationNumber.text = i.ToString();
            yield return new WaitForSeconds(0.1f);
            if(i < bestOfGeneration.Count-1)
                CleanPath(coloredPath);
        }
    }

    private void CleanPath(List<Vector3Int> coloredPath)
    {
        for (int i = 0; i < coloredPath.Count; i++)
        {
            tileMap.SetColor(coloredPath[i], Color.white);
        }
    }

    public List<Vector3Int> ShowPath(Child child, Vector2Int start, Vector2Int finish)
    {
        bool foundFinish = false;
        int i = 0;
        Vector2Int moveOffset = Vector2Int.zero;
        Vector2Int pos = start;
        List<Vector3Int> coloredPath = new List<Vector3Int>();
        while (!foundFinish && i < child.path.Count)
        {
            moveOffset = GAController.DetermineOffset(child.path[i], moveOffset);
            pos += moveOffset;
            if (labGrid.grid[pos.x, pos.y] == 1)
            {
                break;
            }
            Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);
            tileMap.SetColor(tilePos,Color.blue);
            coloredPath.Add(tilePos);
            if(pos.x == finish.x && pos.y == finish.y)
            {
                foundFinish = true;
            }
            i++;
        }
        return coloredPath;
    }

}
