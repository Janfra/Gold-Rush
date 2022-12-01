using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class GridUpdateManager : MonoBehaviour
{
    private static Dictionary<GameObject, int> gameObjectIndexDictionary = new Dictionary<GameObject, int>();
    public static GridUpdateManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Started as an event but since there is going to always only be one manager to avoid index repetition decided to make it a singleton
    //public static void RequestIndex(IGridUpdatable objToSet, GameObject thisObj)
    //{
    //    OnIndexRequest?.Invoke(objToSet, thisObj);
    //}

    // Give their index depending on if they are the same gameobject or not.
    public int GetIndex(GameObject thisObj)
    {
        if (gameObjectIndexDictionary.TryGetValue(thisObj, out int Index))
        {
            //Debug.Log($"GridUpdateIndex {Index} assigned to {thisObj.name}");
            return Index;
        }
        else
        {
            int newIndex = gameObjectIndexDictionary.Count;
            gameObjectIndexDictionary.Add(thisObj, newIndex);
            //Debug.Log($"GridUpdateIndex {newIndex} created for {thisObj.name}");
            return newIndex;
        }
    }
}
