using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;
    public List<Pool> poolsList;
    public Dictionary<PoolObjName, Queue<GameObject>> poolDictionary;

    // NOTE: May try and create an interface for all spawnable objects to then create a generic function that returns their script to avoid getcomponent.

    #region Pools

    /// <summary>
    /// List of objects available to the object pool.
    /// </summary>
    [System.Serializable]
    public enum PoolObjName
    {
        [InspectorName("Resource/GoldNugget")]
        GoldNugget,

        [InspectorName("Resource/GoldBlock")]
        GoldBlock,

        [InspectorName("Entities/Player")]
        Player,

        [InspectorName("Entities/Enemy")]
        BasicEnemy,

    }

    /// <summary>
    /// Pools of objects that will be instantiated. Sets the key, prefabs and max size. Can be customized.
    /// </summary>
    [System.Serializable]
    public class Pool
    {
        public PoolObjName tag;
        public GameObject prefab;
        public int size;
    }

    #endregion

    /// <summary>
    /// In case of not wanting it to be a singleton and be a component, take Instance out.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Creates the dictionary and populate it following pool class information.
    /// </summary>
    void Start()
    {
        poolDictionary = new Dictionary<PoolObjName, Queue<GameObject>>();

        // Debugging counter
        byte poolCount = 0;
        foreach (var pool in poolsList)
        {
            // Check for no repeated pools
            if (poolDictionary.ContainsKey(pool.tag))
            {
                Debug.LogError($"Pool {pool.tag} is repeated. There can only be 1 pool per prefab. Check pool number: {poolCount}");
                return;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            // Instantiate prefab, add it to the queue and give it a name for debug.
            for (int i = 0; i < pool.size; i++)
            {
                GameObject prefab = Instantiate(pool.prefab);
                prefab.SetActive(false);
                objectPool.Enqueue(prefab);
                prefab.name = $"{pool.tag} #{i}";
                prefab.transform.SetParent(transform);
            }

            // Populate the dictionary with the created queue.
            poolDictionary.Add(pool.tag, objectPool);
            poolCount++;
        }
    }

    /// <summary>
    /// Grab an existent pool object and spawn it at the given position.
    /// </summary>
    /// <param name="tag">Pool key, name of pool trying to be accessed</param>
    /// <param name="pos">Position to be spawned at</param>
    /// <param name="rotation">Rotation for the object when spawned</param>
    /// <returns>Object being spawned</returns>
    public GameObject SpawnFromPool(PoolObjName tag, Vector3 pos, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
            return null;

        GameObject prefab = poolDictionary[tag].Dequeue();

        prefab.transform.SetPositionAndRotation(pos, rotation);
        prefab.SetActive(true);

        // Add it back to the queue to be able to spawn it back again in the future.
        poolDictionary[tag].Enqueue(prefab);
        return prefab;
    }

    /// <summary>
    /// Despawns all objects included in this pool dictionary. NOTE: They are still available for spawning again.
    /// </summary>
    public void DespawnAll()
    {
        foreach (var pool in poolDictionary)
        {
            for (int i = 0; i < pool.Value.Count; i++)
            {
                GameObject pieceToDespawn = pool.Value.Dequeue();
                pieceToDespawn.SetActive(false);
                pool.Value.Enqueue(pieceToDespawn);
            }
        }
    }

    /// <summary>
    /// Despawns all objects from a specific pool included in this pool dictionary. NOTE: They are still available for spawning again.
    /// </summary>
    /// <param name="poolName">Pool key, pool to be despawned</param>
    public void DespawnAll(PoolObjName poolName)
    {
        for (int i = 0; i < poolDictionary[poolName].Count; i++)
        {
            GameObject pieceToDespawn = poolDictionary[poolName].Dequeue();
            pieceToDespawn.SetActive(false);
            poolDictionary[poolName].Enqueue(pieceToDespawn);
        }
    }

}