using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GoldBlockManager : MonoBehaviour
{
    public static event Action<Transform> OnBlockSpawned;

    [SerializeField] private List<Transform> spawningPoints;
    private Dictionary<Vector3, Transform> spawningPointsPositionsDictionary;
    private const int indexOffset = 1;

    private void Awake()
    {
        spawningPointsPositionsDictionary = new Dictionary<Vector3, Transform>();
        foreach(Transform transform in spawningPoints)
        {
            spawningPointsPositionsDictionary.Add(transform.position, transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GoldBlock.OnGoldBlockDisabled += SpawnNewGoldBlock;
        StartCoroutine(SpawnFirstBlock());
    }

    private void SpawnNewGoldBlock(Vector3 currentPos)
    {
        Debug.Log("Spawning new gold block");
        Transform currentSpawnPoint;
        if(spawningPointsPositionsDictionary.TryGetValue(currentPos, out currentSpawnPoint))
        {
            spawningPoints.Remove(currentSpawnPoint);
            SpawnBlockAtRandomPoint();
            spawningPoints.Add(currentSpawnPoint);
        }
        else
        {
            Debug.Log($"{currentPos} does not exist in the stored spawning points! Spawning at random location.");
            SpawnBlockAtRandomPoint();
        }
    }

    private void SpawnBlockAtRandomPoint()
    {
        Transform spawningPoint = spawningPoints[GetRandomPoint()];
        GameObject goldBlock = ObjectPooler.Instance.SpawnFromPool(ObjectPooler.PoolObjName.GoldBlock, spawningPoint.position, spawningPoint.rotation);
        goldBlock.SetActive(true);
        OnBlockSpawned?.Invoke(goldBlock.transform);
    }

    private int GetRandomPoint()
    {
        return Random.Range(0, Mathf.Clamp(spawningPoints.Count - indexOffset, 0, spawningPoints.Count));
    }

    private IEnumerator SpawnFirstBlock()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnBlockAtRandomPoint();
    }

    private void OnDrawGizmos()
    {
        foreach(Transform transform in spawningPoints)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 1f);
        }
    }
}
