using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UI;

public class Pathfinder : MonoBehaviour
{
    public Text datastructureText;
    public Text totalTimeText;
    public Text averageTimeText;
    public Transform seeker, target;
    public bool useHeap;
    long _totalRunTime = 0;

    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
        
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            
        }
    }

    void Start()
    {
        int count = 0;
        float startPos = seeker.position.z;
        Vector3 newPostion = seeker.position;

        while (newPostion.z >= -startPos)
        {
            Instantiate(Resources.Load("Seeker") as GameObject, newPostion, Quaternion.identity);
            FindPath(newPostion, target.position);
            newPostion = new Vector3(newPostion.x, newPostion.y, newPostion.z - 8);
            count++;
        }

        string dataStructure = (useHeap) ? "Heap" : "Array";
        float averageRunTime = Mathf.Round((_totalRunTime / (float)count) * 100f)/100f;
        datastructureText.text += dataStructure;
        totalTimeText.text += _totalRunTime.ToString() + "ms";
        averageTimeText.text += averageRunTime.ToString() + "ms";
        print("Data Structure: " + dataStructure);
        print("Total Run Time: " + _totalRunTime + " ms");
        print("Averge Run Time: " + averageRunTime + " ms");
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch algorithmStopwatch = new Stopwatch();
        algorithmStopwatch.Start();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        HashSet<Node> closedSet = new HashSet<Node>(); // Set of nodes already evaluated

        if (useHeap)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                //UnityEngine.Debug.Log(openSet.Count);
                //Node currentNode = openSet[0];
                Node currentNode = openSet.RemoveFirst();
                /*
                for (int i = 1; i < openSet.Count; i++) // Find the node with the min fCost
                {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }
                  openSet.Remove(currentNode);
                */

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    algorithmStopwatch.Stop();
                    _totalRunTime += algorithmStopwatch.ElapsedMilliseconds;
                    //print("Path found: " + algorithmStopwatch.ElapsedMilliseconds + " ms");
                    RetracePath(startNode, targetNode);
                    return;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour); //TODO: This is part of heap. it was a mistake
                        }
                    }
                }
            }
        }
        else
        {
            List<Node> openSet = new List<Node>(); // Set of nodes to be evaluated
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
             
                for (int i = 1; i < openSet.Count; i++) // Find the node with the min fCost
                {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    algorithmStopwatch.Stop();
                    _totalRunTime += algorithmStopwatch.ElapsedMilliseconds;
                    //print("Path found: " + algorithmStopwatch.ElapsedMilliseconds + " ms");
                    RetracePath(startNode, targetNode);
                    return;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;
        grid.DrawPath();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX); 
    }
	
}
