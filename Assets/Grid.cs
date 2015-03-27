using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public bool onlyDisplayPathGizmos;
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public List<Node> path;
    public GameObject pathCube;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        Debug.Log("Starting");
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        // Populate the grid with nodes
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x,y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighnours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkY >= 0 && checkX < gridSizeX && checkY < gridSizeY)
                {
                    neighnours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighnours;
    }

    public void DrawPath()
    {
        if (path != null)
        {
            foreach (Node n in path)
            {
                Instantiate(pathCube, n.worldPosition, Quaternion.identity);
            }
        }
        
    }

   
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (onlyDisplayPathGizmos)
        {
            if (path != null)
            {
                foreach (Node n in path)
                {
                    //Gizmos.color = Color.black;
                    //Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                    
                }
            }
        }
        else
        {

            if (grid != null)
            {
                Node playerNode = NodeFromWorldPoint(player.position);
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    if (path != null)
                    {
                        if (path.Contains(n))
                        {
                            Gizmos.color = Color.black;
                        }
                    }
                    if (n == playerNode)
                    {
                        Gizmos.color = Color.cyan;
                    }

                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
    }	
}
