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

    [SerializeField]
    private float craterDepth = 0.02f;
    [SerializeField] 
    private int craterRadius = 3;

    private void OnCollisionEnter(Collision other)
    {
        if (IsServer && !_exploded)
        {
            _exploded = true;
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            
            explosion.GetComponent<NetworkObject>().Spawn();

            if (other.collider is TerrainCollider)
            {
                Vector3 collisionPoint = other.GetContact(0).point;
                TerraformCrater(collisionPoint);
                SynchronizeTerrainClientRpc(collisionPoint);
            }
            
            GetComponent<NetworkObject>().Despawn();
        }
    }
    
    private void TerraformCrater(Vector3 worldPoint)
    {
        Terrain terrain = Terrain.activeTerrain;

        TerrainData terrainData = terrain.terrainData;
        int terrainWidth = terrainData.heightmapResolution;
        int terrainHeight = terrainData.heightmapResolution;
        
        //Convert world to terrain heights position
        //TODO maak hier een functie van die een vector2 ofso returned.
        Vector3 terrainPosition = terrain.gameObject.transform.position;
        int xPosTerrainHeights = (int) ((worldPoint.x - terrainPosition.x)/ terrainData.size.x * terrainWidth);
        int zPosTerrainHeights = (int) ((worldPoint.z - terrainPosition.z)/ terrainData.size.z * terrainHeight);
    
        float[,] modifiedHeights = terrainData.GetHeights(
            xPosTerrainHeights - craterRadius, 
            zPosTerrainHeights - craterRadius, 
            craterRadius * 2, 
            craterRadius * 2);

        Vector2 craterCenterIndex = new Vector2(craterRadius, craterRadius);
        double greatestDistance = Math.Sqrt(Math.Pow(craterRadius, 2) + Math.Pow(craterRadius, 2));
        
        for (int x = 0; x < craterRadius *2; x++)
        {
            for (int z = 0; z < craterRadius*2; z++)
            {
                float distanceFromCenter = Vector2.Distance(craterCenterIndex, new Vector2(x,z));
                Debug.Log("x: "+x+" y: "+ z+" dist: "+ distanceFromCenter);

                if (distanceFromCenter <= craterRadius)
                {
                    double depthChange = craterDepth - distanceFromCenter / craterRadius * craterDepth;
                    modifiedHeights[x, z] -= (float) depthChange;
                }
            }
        }
        
        /*Debug.Log(modifiedHeights[zPosTerrainHeights, xPosTerrainHeights]);
        modifiedHeights[zPosTerrainHeights, xPosTerrainHeights] -= craterDepth;
        Debug.Log(modifiedHeights[zPosTerrainHeights, xPosTerrainHeights]);*/
        
        terrainData.SetHeights(
            xPosTerrainHeights - craterRadius,
            zPosTerrainHeights - craterRadius,
            modifiedHeights);
    }

    [ClientRpc]
    private void SynchronizeTerrainClientRpc(Vector3 worldPoint)
    {
        TerraformCrater(worldPoint);
    }
}

