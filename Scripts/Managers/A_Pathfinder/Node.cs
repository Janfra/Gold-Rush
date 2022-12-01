using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public const int EmptyObjectIndex = -1;

    /// <summary>
    /// Sets if the node is walkable
    /// </summary>
    public bool isWalkable;

    /// <summary>
    /// Sets the world position of this node
    /// </summary>
    public Vector3 worldPos;

    /// <summary>
    /// Sets the X index in the grid of this node
    /// </summary>
    public int gridX;

    /// <summary>
    /// Sets the Y index in the grid of this node
    /// </summary>
    public int gridY;

    /// <summary>
    /// Origin of this node to determining the path to follow.
    /// </summary>
    public Node parent;

    /// <summary>
    /// Unit currently on this node, testing, not working. Remove if neccessary 
    /// </summary>
    public int occupyingObjectIndex;

    /// <summary>
    /// Sets the cost of this node using the distance to the starting node.
    /// </summary>
    public int gCost;

    /// <summary>
    /// Sets the cost of this node using the distance to the target node.
    /// </summary>
    public int hCost;

    /// <summary>
    /// Total cost of this node
    /// </summary>
    public int fCost => gCost + hCost;

    /// <summary>
    /// Sets the type of surface on this node
    /// </summary>
    public A_Pathfind.SurfaceType surfaceType;

    /// <summary>
    /// Index in the heap
    /// </summary>
    public int HeapIndex { get; set; }

    /// <summary>
    /// If the node being compared to has higher priority.
    /// </summary>
    /// <param name="nodeToCompare">Node to be compared to</param>
    /// <returns>Greater: 1, Equal: 0, Less: -1</returns>
    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

    public Node(bool walkableSet, Vector3 newPos, int newGridX, int newGridY, A_Pathfind.SurfaceType surface)
    {
        isWalkable = walkableSet;
        worldPos = newPos;
        gridX = newGridX;
        gridY = newGridY;
        surfaceType = surface;
        occupyingObjectIndex = EmptyObjectIndex;
    }
}
