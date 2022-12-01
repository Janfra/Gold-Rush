using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[RequireComponent(typeof(Pathfind_Grid))]
public class A_Pathfind : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Grid containing all nodes
    /// </summary>
    [SerializeField] private Pathfind_Grid grid;
    [SerializeField] private PathRequestManager requestManager;

    #endregion

    #region Penalties & Surfaces

    /// <summary>
    /// Class to store and display in the inspector penalties available.
    /// </summary>
    [System.Serializable]
    private class SurfacePenalties
    {
        public SurfaceType surfaceType;
        public int penaltyValue;
    }

    [SerializeField] private SurfacePenalties[] listOfPenalties;

    [System.Serializable]
    public enum SurfaceType
    {
        None,
        Grass,
        Bush,
        MovingEntity,

    }

    #endregion

    private void OnValidate()
    {
        if (listOfPenalties.Length == 0) 
        {
            listOfPenalties = new SurfacePenalties[Enum.GetValues(typeof(SurfaceType)).Length];
        }
    }

    private void Awake()
    {
        if (grid == null)
        {
            grid = GetComponent<Pathfind_Grid>();
        }
        if(requestManager == null)
        {
            requestManager = GetComponent<PathRequestManager>();
        }
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos, A_Pathfind.SurfaceType[] surfacePenalties, IGridUpdatable objectRequesting)
    {
        StartCoroutine(FindPath(startPos, targetPos, surfacePenalties, objectRequesting));
    }

    /// <summary>
    /// Finds the closest path to the target node.
    /// </summary>
    /// <param name="startPos">Starting position</param>
    /// <param name="targetPos">Desired position to get to</param>
    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, A_Pathfind.SurfaceType[] surfacePenalties, IGridUpdatable objectRequesting)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        
        //Added a check in case it is possible to find path next to current position, as the player standing next to unwalkable areas could break pathfind
        if (!targetNode.isWalkable)
        {
            Node[] walkableNeighbours = GetWalkableNeighbour(targetNode);
            if (walkableNeighbours.Length > 0)
            {
                targetNode = GetClosestNode(targetNode, walkableNeighbours);
            }
        }
        // This one is just in case
        if (!startNode.isWalkable)
        {
            Node[] walkableNeighbours = GetWalkableNeighbour(startNode);
            if (walkableNeighbours.Length > 0)
            {
                startNode = GetClosestNode(startNode, walkableNeighbours);
            }
        }

        if (startNode.isWalkable && targetNode.isWalkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            // Hashset lets you quickly determine whether an object is already in the set or not.
            // All elements are unique. Doesnt keep the order in which elements were added. Faster than list. No index to search, only enumerator and create to list.
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while(openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if(currentNode == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    // TEST //
                    if(neighbour.occupyingObjectIndex != Node.EmptyObjectIndex && neighbour.occupyingObjectIndex != objectRequesting.GridUpdateIndex)
                    {
                        closedSet.Add(neighbour);
                    }
                    //
                    if(!neighbour.isWalkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    
                    // Check if the penalties are appliable
                    if (neighbour.surfaceType != SurfaceType.None && surfacePenalties.Contains(neighbour.surfaceType))
                    {
                        newMovementCostToNeighbour += listOfPenalties[(int)neighbour.surfaceType].penaltyValue;

                        //Debug.Log($"Penalty added: {listOfPenalties[(int)neighbour.surfaceType].penaltyValue}, penalty index: {(int)neighbour.surfaceType}, surface type: {neighbour.surfaceType}");
                    }
                    if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
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
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }

        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    /// <summary>
    /// Returns the waypoints to follow to get to target
    /// </summary>
    /// <param name="startNode">Starting pos</param>
    /// <param name="endNode">Target pos</param>
    /// <returns>Array of Vector3 with pos to follow</returns>
    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    /// <summary>
    /// Takes unnecessary positions to create a path to follow
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            // Had to add a check for unwalkable nodes to avoid path going into unwalkable areas after simplifying
            if (directionNew != directionOld || !AreNeighboursWalkable(path[i]))
            {
                waypoints.Add(path[i].worldPos);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    /// <summary>
    /// Distance in between two nodes.
    /// </summary>
    /// <param name="nodeA">Starting node</param>
    /// <param name="nodeB">Target node</param>
    /// <returns>Distance in between the nodes for the H cost</returns>
    private int GetDistance(Node nodeA, Node nodeB)
    {
        // absolute value just makes negative numbers positive, useful for checking distances.
        int disX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int disY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        // 14 is the diagonal cost in a square grid and 10 the horizontal/vertical cost. Use the lowest distance for the diagonal. For visual min 14 https://www.youtube.com/watch?v=mZfyt03LDH4
        if (disX > disY)
        {
            return 14 * disY + 10 * (disX - disY);
        }
        else
        {
            return 14 * disX + 10 * (disY - disX);
        }
    }

    #region Fixes
    private Node[] GetWalkableNeighbour(Node currentUnwalkable)
    {
        List<Node> potentialNewTargets = new List<Node>();
        foreach (Node n in grid.GetNeighbours(currentUnwalkable))
        {
            if (n.isWalkable)
                potentialNewTargets.Add(n);
        }
        return potentialNewTargets.ToArray();
    }

    private bool AreNeighboursWalkable(Node node)
    {
        foreach(Node n in grid.GetNeighbours(node))
        {
            if (!n.isWalkable)
            {
                return false;
            }
        }
        return true;
    }

    private Node GetClosestNode(Node target, Node[] options)
    {
        // Random high number
        int currentNDistance = 1000;
        Node closestNode = null;
        foreach(Node n in options)
        {
            int nDistance = GetDistance(target, n);
            if(nDistance < currentNDistance)
            {
                currentNDistance = nDistance;
                closestNode = n;
            }
        }
        return closestNode;
    }

    #endregion
}
