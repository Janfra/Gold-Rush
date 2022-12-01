using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathRequestManager : MonoBehaviour
{
    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest currentPathRequest;

    // Singleton
    public static PathRequestManager instance;
    [SerializeField] private A_Pathfind pathfinding;

    private bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        if(pathfinding == null)
        {
            pathfinding = GetComponent<A_Pathfind>();
        }
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, A_Pathfind.SurfaceType[] surfaceAppliable, Action<Vector3[], bool> callback, IGridUpdatable objectRequesting)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, surfaceAppliable, callback, objectRequesting);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.surfacePenalties, currentPathRequest.objectRequesting);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public A_Pathfind.SurfaceType[] surfacePenalties;
        public Action<Vector3[], bool> callback;
        public IGridUpdatable objectRequesting;

        public PathRequest(Vector3 start, Vector3 end, A_Pathfind.SurfaceType[] surfaceAppliable, Action<Vector3[], bool> newCallback, IGridUpdatable obj)
        {
            pathStart = start;
            pathEnd = end;
            surfacePenalties = surfaceAppliable;
            callback = newCallback;
            objectRequesting = obj;
        }
    }
}
