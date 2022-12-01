using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfind_Grid : MonoBehaviour
{
    #region Variables

    [SerializeField] private bool DrawGizmos;

    /// <summary>
    /// Layers of the unwalkable nodes.
    /// </summary>
    [SerializeField] private LayerMask unwalkableMask;

    /// <summary>
    /// Total size of the 2D grid.
    /// </summary>
    [SerializeField] private Vector2 gridWorldSize;

    /// <summary>
    /// Size of each node in the grid.
    /// </summary>
    [SerializeField] private float nodeRadius;

    /// <summary>
    /// Grid of nodes that store the information of each position in the grid.
    /// </summary>
    Node[,] grid;


    /// <summary>
    /// Total size covered by a node.
    /// </summary>
    private float nodeDiameter;

    /// <summary>
    /// Amount of nodes in the X and Y cords/index of the 2D array/grid.
    /// </summary>
    private int gridSizeX, gridSizeY;

    /// <summary>
    /// Total size of the grid nodes
    /// </summary>
    public int MaxSize => gridSizeX * gridSizeY;

    #endregion

    #region Tag/Surface

    /// <summary>
    /// Starting point of the custom tags 0-7.
    /// </summary>
    private const int customTagStart = 7;

    /// <summary>
    /// Adds one to the surface index to align it with the tags.
    /// </summary>
    private const int surfaceIndexOffset = 1;

    private Dictionary<string, A_Pathfind.SurfaceType> tagToSurfaceDictionary;

    #endregion

    #region Dynamic Update

    public static event Action<Vector3, int, int> OnUpdateGrid;
    public static event Action<Vector3, int, int, int> OnDynamicUpdate;
    public static event Action AfterUpdateGrid;
    [SerializeField] private LayerMask ignoreOnUpdate;

    #endregion

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;

        // To get how many nodes will fit in X or Y first divide the total size by the size of each node
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        // Create a dictionary to convert tags strings into surface enum
        tagToSurfaceDictionary = new Dictionary<string, A_Pathfind.SurfaceType>();
        for(int i = customTagStart; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
        {
            tagToSurfaceDictionary.Add(UnityEditorInternal.InternalEditorUtility.tags[i], (A_Pathfind.SurfaceType) (i + surfaceIndexOffset) - customTagStart);
            UnityEngine.Debug.Log($"Surface {(A_Pathfind.SurfaceType)(i + surfaceIndexOffset) - customTagStart} added to dictionary with key {UnityEditorInternal.InternalEditorUtility.tags[i]}");
        }

        CreateGrid();
        OnUpdateGrid += UpdatePosition;
        OnDynamicUpdate += UpdatePosition;
    }

    /// <summary>
    /// Generates the grid by populating it with nodes, starting at the bottom left.
    /// </summary>
    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        // The center of the current position, then subtract half the x size to get the border on the left, then substract half the total y to get to the bottom.
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Save the current position and then check if is walkable by checking if a collision happens
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool isWalkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);

                A_Pathfind.SurfaceType surfaceType = A_Pathfind.SurfaceType.None;
                if (isWalkable)
                {
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit, ~ignoreOnUpdate))
                    {
                        // If the tag of the object is in the dictionary of surface penalties, set the surface type
                        tagToSurfaceDictionary.TryGetValue(hit.transform.tag, out surfaceType);
                    }
                }

                grid[x, y] = new Node(isWalkable, worldPoint, x, y, surfaceType);
            }
        }
    }

    /// <summary>
    /// Get the neighbour nodes of the given node.
    /// </summary>
    /// <param name="node">Node to check for neighbours</param>
    /// <returns>Neighbours of the node</returns>
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0)
                {
                    // Continue just skips this iteration and keeps going
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    /// <summary>
    /// Gets the node at the given position.
    /// </summary>
    /// <param name="worldPos">Position to check for a node</param>
    /// <returns>The node at the given position, or the closest possible</returns>
    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x / gridWorldSize.x) + 0.5f;
        // worldPos.z because the grid only has X and Y. In this case Z is the Y equivalent.
        float percentY = (worldPos.z / gridWorldSize.y) + 0.5f;

        // Clamp in between 0-1.
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        // Get the total index size and return the index with the % of where the pos is.
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        grid[x, y].isWalkable = !Physics.CheckSphere(grid[x, y].worldPos, nodeRadius, unwalkableMask);

        return grid[x, y];
    }

    #region Dynamic Update

    /// <summary>
    /// Makes the grid check the position given to update it.
    /// </summary>
    /// <param name="posToUpdate">Position to check</param>
    public static void UpdateGrid(Vector3 posToUpdate, int xToUpdate, int yToUpdate)
    {
        OnUpdateGrid?.Invoke(posToUpdate, xToUpdate, yToUpdate);
    }

    public static void DynamicUpdateGrid(Vector3 posToUpdate, int xToUpdate, int yToUpdate, int newNodeUpdateIndex)
    {
        OnDynamicUpdate?.Invoke(posToUpdate, xToUpdate, yToUpdate, newNodeUpdateIndex);
    }

    private void UpdatePosition(Vector3 posToCheck, int xDirectionCheck, int yDirectionCheck)
    {
        Node originNode = NodeFromWorldPoint(posToCheck);
        for(int y = -yDirectionCheck; y < yDirectionCheck; y++)
        {
            for(int x = -xDirectionCheck; x < xDirectionCheck; x++)
            {
                UpdateNode(grid[originNode.gridX + x, originNode.gridY + y]);
            }
        }
        AfterUpdateGrid?.Invoke();
    }

    private void UpdateNode(Node node)
    {
        node.isWalkable = !Physics.CheckSphere(node.worldPos, nodeRadius, unwalkableMask);

        A_Pathfind.SurfaceType updatedSurface = A_Pathfind.SurfaceType.None;
        if (node.isWalkable)
        {
            Ray ray = new Ray(node.worldPos + Vector3.up * 50, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, ~ignoreOnUpdate))
            {
                // If the tag of the object is in the dictionary of surface penalties, set the surface type
                tagToSurfaceDictionary.TryGetValue(hit.transform.tag, out updatedSurface);
            }
        }
        node.surfaceType = updatedSurface;
    }

    private void UpdatePosition(Vector3 posToCheck, int xDirectionCheck, int yDirectionCheck, int newNodeUpdateIndex)
    {
        Node originNode = NodeFromWorldPoint(posToCheck);
        for (int y = -yDirectionCheck; y < yDirectionCheck; y++)
        {
            for (int x = -xDirectionCheck; x < xDirectionCheck; x++)
            {
                UpdateNode(grid[originNode.gridX + x, originNode.gridY + y], newNodeUpdateIndex);
            }
        }
    }

    private void UpdateNode(Node node, int newNodeUpdateIndex)
    {
        node.occupyingObjectIndex = newNodeUpdateIndex;
        node.isWalkable = !Physics.CheckSphere(node.worldPos, nodeRadius, unwalkableMask);

        A_Pathfind.SurfaceType updatedSurface = A_Pathfind.SurfaceType.None;
        if (node.isWalkable)
        {
            Ray ray = new Ray(node.worldPos + Vector3.up * 50, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, ~ignoreOnUpdate))
            {
                // If the tag of the object is in the dictionary of surface penalties, set the surface type
                tagToSurfaceDictionary.TryGetValue(hit.transform.tag, out updatedSurface);
            }
        }
        node.surfaceType = updatedSurface;
    }

    #endregion

    #region Not used, for now

    //private void BlurPenalty(int blurSize)
    //{
    //    int kernelSize = blurSize * 2 + 1;
    //    int kernelExtends = (kernelSize - 1) / 2;

    //    int[,] penaltyHorizontalPass = new int[gridSizeX, gridSizeY];
    //    int[,] penaltyVerticalPass = new int[gridSizeX, gridSizeY];

    //    for (int y = 0; y < gridSizeY; y++)
    //    {
    //        for (int x = -kernelExtends; x <= kernelExtends; x++)
    //        {
    //            int sampleX = Mathf.Clamp(x, 0, kernelExtends);
    //            penaltyHorizontalPass[0, y] += grid[sampleX, y].
    //        }
    //        for (int x = 1; x < gridSizeX; x++)
    //        {
    //            int removeIndex = Mathf.Clamp(x - kernelExtends - 1, 0, gridSizeX);
    //            int addIndex = Mathf.Clamp(x + kernelExtends, 0, gridSizeX - 1);

    //            penaltyHorizontalPass[x, y] = penaltyHorizontalPass[x-1, y]
    //        }
    //    }
    //} 

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && DrawGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.isWalkable ? Color.white : Color.red);
                if(n.surfaceType != A_Pathfind.SurfaceType.None && n.surfaceType != A_Pathfind.SurfaceType.Grass)
                {
                    Gizmos.color = Color.magenta;
                }
                Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }

    #endregion
}
