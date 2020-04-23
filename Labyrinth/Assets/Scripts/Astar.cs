using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Astar
{


    public static bool FindPath(LabGrid labGrid, Vector2Int start, Vector2Int finish)
    {
        int[,] grid = labGrid.grid;
        MinHeap nodes = new MinHeap();
        Node startNode = new Node(null,start.x,start.y);
        Evaluate(startNode, finish);
        nodes.Insert(startNode);

        int[,] visited = new int[labGrid.width, labGrid.height];
        visited[startNode.x, startNode.y] = 1;

        bool found = false;
        Node node = new Node();
        while(nodes.list.Count > 0 && !found)
        {
            node = nodes.PullMin();
            if(node.x == finish.x && node.y == finish.y)
            {
                found = true;
                break;
            }
            List<Node> neighbours = new List<Node>();
            PopulateNeighbours(node, neighbours);

            foreach (Node item in neighbours) 
            {
                if(item.x >= 0 && item.y >= 0 && item.x < labGrid.width && item.y < labGrid.height && (grid[item.x,item.y] == 0 || grid[item.x, item.y] == 2))
                {
                    if (visited[item.x, item.y] == 0)
                    {
                        Evaluate(item,finish);
                        GiveWeight(item);
                        nodes.Insert(item);
                        visited[item.x, item.y] = 1;
                    }
                }
            }
        }
        if (found)
            return true;
        else
            return false;

    }

    private static void PopulateNeighbours(Node node, List<Node> neighbours) // Komşuları belirleme
    {
        int x = node.x;
        int y = node.y;

        neighbours.Add(new Node(node, x - 1, y));  //1
        //neighbours.Add(new Node(node, x - 1, y + 1)); //2
        neighbours.Add(new Node(node, x, y + 1)); //3
        //neighbours.Add(new Node(node, x + 1, y + 1)); //4
        neighbours.Add(new Node(node, x + 1, y)); //5
        //neighbours.Add(new Node(node, x + 1, y - 1)); //6
        neighbours.Add(new Node(node, x, y - 1)); //7
        //neighbours.Add(new Node(node, x - 1, y - 1)); //8

        // ____ ____ ____ 
        //|__8_|__7_|__6_|
        //|__1_|__x_|__5_|
        //|__2_|__3_|__4_|

        //Noktanın etrafındaki 8 diğer komşu nokta

    }

    private static void Evaluate(Node node, Vector2Int finish)
    {
        long distance = Convert.ToInt64(Math.Sqrt((finish.x - node.x) * (finish.x - node.x) + (finish.y - node.y) * (finish.y - node.y)));
        node.h = distance;
    }

    private static void GiveWeight(Node item)
    {
        item.g = item.parent.g + 1;
    }

}

public class Node // Nokta (Piksel)
{
    public Node parent { get; set; } // Parent noktası

    public int x { get; set; } // X koordinatı
    public int y { get; set; } // Y koordinatı
    public long h { get; set; } // h(n) değeri
    public long g { get; set; } // g(n) değeri
    public long f // f(n) değeri
    {
        get
        {
            return g + h;
        }
    }

    public Node(Node parent, int x, int y) // Constructor
    {
        this.parent = parent;
        this.x = x;
        this.y = y;
    }

    public Node()
    {

    }
}
