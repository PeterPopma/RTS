using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] pfSpawnObjects;
    [SerializeField] Transform rootTransform;
    [SerializeField] int numItems = 0;
    [SerializeField] float minX = -1500;
    [SerializeField] float maxX = 1500;
    [SerializeField] float minZ = -1500;
    [SerializeField] float maxZ = 1500;

    public void SpawnTrees()
    {
        for (int i = 0; i < numItems; i++)
        {
            SpawnNewObject();
        }
    }

    private void SpawnNewObject()
    {
        int objectIndex = Random.Range(0, pfSpawnObjects.Length);
        int triesLeft = numItems * 10;

        Vector3 spawnPosition;
        do {
            spawnPosition = new Vector3(Random.value * (maxX - minX) + minX, 2000, Random.value * (maxZ - minZ) + minZ);
            float Yoffset = Terrain.activeTerrain.SampleHeight(spawnPosition);
            spawnPosition.y = Yoffset;
            triesLeft--;
        } while (!CheckNoOtherObjectsNearby(spawnPosition) && triesLeft>0);
        GameObject newObject = Instantiate(pfSpawnObjects[objectIndex], spawnPosition,
                                                Quaternion.identity); 

        newObject.transform.parent = rootTransform;
        Game.Instance.Trees.Add(newObject);
    }

    private bool CheckNoOtherObjectsNearby(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 20);
        foreach (var collider in colliders)
        {
            if( !collider.gameObject.name.StartsWith("pfTree") &&
                !collider.gameObject.name.StartsWith("Terrain"))
            {
                return false;
            }
        }
        
        return true;
    }

}
