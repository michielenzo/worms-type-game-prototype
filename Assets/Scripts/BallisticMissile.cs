using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BallisticMissile : NetworkBehaviour
{
    [SerializeField] 
    private GameObject explosionPrefab;

    // This makes sure there are no double explosions.
    private bool _exploded;

    private void OnCollisionEnter(Collision other)
    {
        if (IsServer && !_exploded)
        {
            _exploded = true;
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            
            explosion.GetComponent<NetworkObject>().Spawn();
            
            GetComponent<NetworkObject>().Despawn();

            if (other.collider is TerrainCollider)
            {
                Vector3 collisionPoint = other.GetContact(0).point;
                RaiseTerrain(collisionPoint);
            }
        }
    }

    private void RaiseTerrain(Vector3 worldPoint)
    {
        Terrain terrain = Terrain.activeTerrain;

        TerrainData terrainData = terrain.terrainData;
        int terrainWidth = terrainData.heightmapResolution;
        int terrainHeight = terrainData.heightmapResolution;
        
        //Convert world to terrain heights position
        //TODO maak hier een functie van die een vector2 ofso returned.
        int xPosTerrain = (int) (worldPoint.x / terrainData.size.x * terrainWidth);
        int zPosTerrain = (int) (worldPoint.z / terrainData.size.z * terrainHeight);
    
        float[,] modifiedHeights = terrainData.GetHeights(0,0,terrainWidth, terrainHeight);

        modifiedHeights[zPosTerrain, xPosTerrain] = 0.01f;
        
        terrainData.SetHeights(0,0, modifiedHeights);
    }
}

